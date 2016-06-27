using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TextAnalyticsBot.Api;
using TextAnalyticsBot.DataModel;

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
            replyMessage.SetBotPerUserInConversationData("sentimentScore", result[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score);
            return replyMessage;
        }

        private string FormatResultMessage(Dictionary<TextAnalyticsResultType, TextAnalyticsResult> input)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Language : {input[TextAnalyticsResultType.Languages].Documents.FirstOrDefault().DetectedLanguages.FirstOrDefault().Name} {Environment.NewLine}");
            sb.AppendFormat($"Key Phrases are : { string.Join(",", input[TextAnalyticsResultType.KeyPhrases].Documents.FirstOrDefault().KeyPhrases)} {Environment.NewLine}");
            sb.AppendFormat($"Sentiment is {input[TextAnalyticsResultType.Sentiment].Documents.FirstOrDefault().Score} {Environment.NewLine}");
            return sb.ToString();
        }
    }
}