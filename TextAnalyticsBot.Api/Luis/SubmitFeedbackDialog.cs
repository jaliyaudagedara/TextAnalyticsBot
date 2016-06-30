using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAnalyticsBot.Api.TextAnalytics;
using TextAnalyticsBot.DataModel;
using TextAnalyticsBot.DataModel.Feedback;
using TextAnalyticsBot.DataModel.Luis;

namespace TextAnalyticsBot.Api.Luis
{
    [LuisModel("9867892d-d767-4795-b15c-fe07d1f35b3d", "bf82958d57934b0a9b79d2eb6d2687a2")]
    [Serializable]
    public class SubmitFeedbackDialog : LuisDialog<Feedback>
    {
        private readonly BuildFormDelegate<Feedback> SubmitFeedbackForm;

        public SubmitFeedbackDialog(BuildFormDelegate<Feedback> submitFeedbackForm)
        {
            this.SubmitFeedbackForm = submitFeedbackForm;
        }

        //public string GetMessageFromLuisQueryData(LuisQueryData luisQueryData)
        //{
        //    string replyMessageText = "I am sorry, I didn't quite catch that.";

        //    if (luisQueryData.Intents == null && luisQueryData.TopScoringIntent != null)
        //    {
        //        switch (luisQueryData.TopScoringIntent.Intent)
        //        {
        //            case "Welcome":
        //                replyMessageText = "Hi, I am your bot!";
        //                break;
        //            case "GetEventDetails":
        //                replyMessageText = "Something";
        //                break;
        //            default:
        //                replyMessageText = "I am sorry, I didn't quite catch that.";
        //                break;
        //        }
        //    }
        //    else if (luisQueryData.Intents.Count() > 0)
        //    {
        //        switch (luisQueryData.Intents.FirstOrDefault().Intent)
        //        {
        //            case "Welcome":
        //                replyMessageText = "Hi, I am your bot!";
        //                break;
        //            case "GetEventDetails":
        //                replyMessageText = "Something";
        //                break;
        //            default:
        //                replyMessageText = "I am sorry, I didn't quite catch that.";
        //                break;
        //        }
        //    }
        //    return replyMessageText;
        //}

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I am sorry, I didn't quite catch that. Please try again.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            string contextId = string.Empty;
            context.PerUserInConversationData.TryGetValue<string>("contextId", out contextId);
            
            if (!string.IsNullOrEmpty(contextId))
            {
                context.PerUserInConversationData.RemoveValue("contextId");
            }

            await context.PostAsync("Hi there, I am your Text Analytics Bot. Currently I am still under development. Anyway let's give a try. What do you want me to do? (Right now, I can only accept feedbacks for events/forums!)!");
            context.Wait(MessageReceived);
        }

        //List<EntityRecommendation> Entities = new List<EntityRecommendation>();

        [LuisIntent("GetEventDetails")]
        public async Task GetEventDetails(IDialogContext context, LuisResult result)
        {
            //foreach (var item in result.Entities)
            //{
            //    entities.Add(new EntityRecommendation()
            //    {
            //        Type = item.Type,
            //        Entity = item.Entity
            //    });
            //}

            string contextId = string.Empty;
            context.PerUserInConversationData.TryGetValue<string>("contextId", out contextId);

            LuisQueryData luisQueryData;
            if (string.IsNullOrEmpty(contextId))
            {
                luisQueryData = await LuisClient.SendRequest(result.Query);
            }
            else
            {
                luisQueryData = await LuisClient.SendRequest(result.Query, contextId);
            }

            if (luisQueryData != null && luisQueryData.Dialog != null && luisQueryData.Dialog.Status == "Question")
            {
                string replyMessageText = luisQueryData.Dialog.Prompt;
                await context.PostAsync(replyMessageText);

                context.PerUserInConversationData.SetValue("contextId", luisQueryData.Dialog.ContextId);
                context.Wait(MessageReceived);
            }
            else if (luisQueryData != null && luisQueryData.Dialog != null && luisQueryData.Dialog.Status == "Finished")
            {
                StringBuilder sb = new StringBuilder();

                var parameters = luisQueryData.TopScoringIntent.Actions.FirstOrDefault().Parameters;
                foreach (var item in parameters)
                {
                    sb.AppendLine(", ");
                    sb.AppendLine($"{item.Value.FirstOrDefault().Type} : {item.Value.FirstOrDefault().Entity}");
                }
                sb.Remove(0, 1);

                await context.PostAsync(sb.ToString());
                PromptDialog.Confirm(context, EventInformationConfirmed, "Are the event details correct?", "Didn't get that!", promptStyle: PromptStyle.None);
            }
            else
            {
                string replyMessageText = "I am sorry, I can't recognize that. Please try again.";
                await context.PostAsync(replyMessageText);
                context.Wait(MessageReceived);
            }
        }

        private async Task EventInformationConfirmed(IDialogContext context, IAwaitable<bool> result)
        {
            var isConfirmed = await result;
            if (isConfirmed)
            {
                await context.PostAsync("Noted. you can now leave your feedback.");
                context.Wait(FeedbackReceived);
            }
            else
            {
                //var feedbackForm = new FormDialog<Feedback>(new Feedback() { }, this.SubmitFeedbackForm, FormOptions.None, entities: Entities);
                //context.Call<Feedback>(feedbackForm, EventInformationNotConfirmed);

                await context.PostAsync("Oh, let's start over.");
                context.Wait(MessageReceived);
            }
        }

        private async Task EventInformationNotConfirmed(IDialogContext context, IAwaitable<Feedback> result)
        {
            Feedback f = await result;
            context.Wait(MessageReceived);
        }

        private async Task FeedbackReceived(IDialogContext context, IAwaitable<Message> result)
        {
            var message = await result;

            TextAnalyticsMessage textAnalyticsMessage = new TextAnalyticsMessage();
            textAnalyticsMessage.Documents.Add(new TextAnalyticsDocument()
            {
                Id = Guid.NewGuid().ToString(),
                Text = message.Text
            });

            Dictionary<TextAnalyticsResultType, TextAnalyticsResult> textAnalyticsResult = await TextAnalyticsClient.SendRequest("https://westus.api.cognitive.microsoft.com", "f47633a4d0a74abb9282a9cc22a79925", 1, textAnalyticsMessage);

            Message replyMessage = message.CreateReplyMessage($"{FormatResultMessage(textAnalyticsResult)}");
            replyMessage.SetBotPerUserInConversationData("sentimentScore", textAnalyticsResult[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score);
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceived);
        }

        private string FormatResultMessage(Dictionary<TextAnalyticsResultType, TextAnalyticsResult> input)
        {
            StringBuilder sb = new StringBuilder();

            List<string> keyPhrases = input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (keyPhrases != null && keyPhrases.Count > 0)
            {
                sb.AppendFormat($"Key Phrases are : { string.Join(",", input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases)} {Environment.NewLine}.");
            }

            double sentimentScore = input[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score;
            bool isPositive = sentimentScore < 0.5 ? false : true;
            sb.AppendLine(isPositive ? "It seems that you are satisfied with the event content. I am sure organizers would love to see this feedback." : "I am sorry to hear that you not satisfied.");
            sb.AppendLine("Thank you for your feedback.");
            return sb.ToString();
        }
    }
}
