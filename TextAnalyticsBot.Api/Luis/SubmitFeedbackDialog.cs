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
            LuisQueryData luisQueryData = await LuisClient.SendRequest(result.Query);

            string replyMessageText = "Hi there, I am your bot!";

            await context.PostAsync(replyMessageText);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetEventDetails")]
        public async Task GetEventDetails(IDialogContext context, LuisResult result)
        {
            string contextId = "";
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
                var entities = new List<EntityRecommendation>(result.Entities);

                StringBuilder sb = new StringBuilder();

                var parameters = luisQueryData.TopScoringIntent.Actions.FirstOrDefault().Parameters;
                foreach (var item in parameters)
                {
                    sb.AppendLine($"{item.Name} : {item.Value.FirstOrDefault().Entity}");
                }
                await context.PostAsync(sb.ToString());

                PromptDialog.Confirm(context, OnEventInformationConfirmed, "Are the event details correct?", "Didn't get that!", promptStyle: PromptStyle.None);
            }
            else
            {
                string replyMessageText = "I am sorry, I can't recognize that. Please try again.";
                await context.PostAsync(replyMessageText);
                context.Wait(MessageReceived);
            }
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

            Dictionary<TextAnalyticsResultType, TextAnalyticsResult> textAnalyticsResult = await TextAnalyticsClient.MakeRequests("https://westus.api.cognitive.microsoft.com", "f47633a4d0a74abb9282a9cc22a79925", 1, textAnalyticsMessage);

            Message replyMessage = message.CreateReplyMessage($"{FormatResultMessage(textAnalyticsResult)}");
            replyMessage.SetBotPerUserInConversationData("sentimentScore", textAnalyticsResult[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score);
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceived);
        }

        private string FormatResultMessage(Dictionary<TextAnalyticsResultType, TextAnalyticsResult> input)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Language : {input[TextAnalyticsResultType.Languages].Documents.FirstOrDefault().DetectedLanguages.FirstOrDefault().Name} {Environment.NewLine}");

            List<string> keyPhrases = input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (keyPhrases != null && keyPhrases.Count > 0)
            {
                sb.AppendFormat($"Key Phrases are : { string.Join(",", input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases)} {Environment.NewLine}");
            }

            sb.AppendFormat($"Sentiment is {input[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score} {Environment.NewLine}");
            return sb.ToString();
        }

        private  async Task OnEventInformationConfirmed(IDialogContext context, IAwaitable<bool> result)
        {
            var isConfirmed = await result;
            if (isConfirmed)
            {
                await context.PostAsync("Confirmed. Please leave the feedback.");
            }
            context.Wait(FeedbackReceived);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            await context.PostAsync(string.Format("You said {0}. Done", message.Text));
            context.PerUserInConversationData.SetValue("contextId", "");
            context.Wait(MessageReceived);
        }

        private async Task SubmitFeedbackFormComplete(IDialogContext context, IAwaitable<Feedback> result)
        {
            Feedback feedback = null;
            try
            {
                feedback = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            if (feedback != null)
            {
                await context.PostAsync("Your Feedback is for : " + $"{feedback.Country} : {feedback.Event} held on {feedback.DateTime}");
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);

            //await context.PostAsync("Done");
            //context.Wait(MessageReceived);
        }
    }
}
