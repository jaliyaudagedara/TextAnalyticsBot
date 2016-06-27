using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using TextAnalyticsBot.Api;
using TextAnalyticsBot.DataModel;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace TextAnalyticsBot.WebApi
{
    /// <summary>
    /// Class MessagesController.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    //[BotAuthentication()]
    public class MessagesController : ApiController
    {
        private string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private string AccountKey = ConfigurationManager.AppSettings["AccountKey"];
        private int NumLanguages = Convert.ToInt32(ConfigurationManager.AppSettings["NumLanguages"]);

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                var counter = message.GetBotPerUserInConversationData<int>("counter");

                TextAnalyticsMessage textAnalyticsMessage = new TextAnalyticsMessage();
                textAnalyticsMessage.Documents.Add(new TextAnalyticsDocument()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = message.Text
                });

                Dictionary<TextAnalyticsResultType, TextAnalyticsResult> result = await Utility.MakeRequests(BaseUrl, AccountKey, NumLanguages, textAnalyticsMessage);

                Message replyMessage = message.CreateReplyMessage($"{FormatResultMessage(result)}");

                replyMessage.SetBotPerUserInConversationData("counter", counter);

                return replyMessage;
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
            sb.AppendFormat($"Key Phrases are : { string.Join(",", input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases)} {Environment.NewLine}");
            sb.AppendFormat($"Sentiment is {input[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score} {Environment.NewLine}");
            return sb.ToString();
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}