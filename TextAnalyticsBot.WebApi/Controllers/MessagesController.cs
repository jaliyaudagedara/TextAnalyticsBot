using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TextAnalyticsBot.Api;
using TextAnalyticsBot.Api.Luis;
using TextAnalyticsBot.DataModel;
using TextAnalyticsBot.DataModel.Feedback;

namespace TextAnalyticsBot.WebApi
{
    /// <summary>
    /// Class MessagesController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [BotAuthentication()]
    public class MessagesController : ApiController
    {
        private string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private string AccountKey = ConfigurationManager.AppSettings["AccountKey"];
        private int NumLanguages = Convert.ToInt32(ConfigurationManager.AppSettings["NumLanguages"]);

        private static IForm<Feedback> BuildForm()
        {
            var builder = new FormBuilder<Feedback>();

            ActiveDelegate<Feedback> isCountry = (feedback) => string.IsNullOrEmpty(feedback.Country);
            ActiveDelegate<Feedback> isEvent = (feedback) => string.IsNullOrEmpty(feedback.Event);
            ActiveDelegate<Feedback> isDateTime = (feedback) => string.IsNullOrEmpty("HelloWorld");


            return builder
                .Message("Welcome to Feedback bot!")
                .Field(nameof(Feedback.Country))
                .Field(nameof(Feedback.Event))
                .Field(nameof(Feedback.DateTime))
                .Confirm("Need country", isCountry)
                .Message("Thanks for submitting your feedback!")
                .OnCompletion(OnCompletion)
                .Build();
        }

        private static Task OnCompletion(IDialogContext context, Feedback state)
        {
            Feedback f = state;
            return Task.FromResult(0);
        }

        internal static IDialog<Feedback> MakeRoot()
        {
            return Chain.From(() => new SubmitFeedbackDialog(BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return await Conversation.SendAsync(message, MakeRoot);
            }
            else
            {
                return HandleSystemMessage(message);
            }
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

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage("You just pinged me.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
                Message reply = message.CreateReplyMessage("Hello, I am Text Analytics Bot.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
                Message reply = message.CreateReplyMessage("Hello, I am Text Analytics Bot. Please note that I am still under development. But you should be able to send me your text expressions. I will try to analyze.");
                reply.Type = message.Type;
                return reply;
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
                Message reply = message.CreateReplyMessage("Good bye. Hope you had a good time.");
                reply.Type = message.Type;
                return reply;
            }

            return null;
        }
    }
}