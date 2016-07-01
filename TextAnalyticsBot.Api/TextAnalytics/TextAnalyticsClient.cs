using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TextAnalyticsBot.DataModel;

namespace TextAnalyticsBot.Api.TextAnalytics
{
    public class TextAnalyticsClient
    {
        public static async Task<Dictionary<TextAnalyticsResultType, TextAnalyticsResult>> SendRequest(string baseUrl, string accountKey, int noOfLanguages, TextAnalyticsMessage documents)
        {
            Dictionary<TextAnalyticsResultType, TextAnalyticsResult> result = new Dictionary<TextAnalyticsResultType, TextAnalyticsResult>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accountKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(documents));

                var uri = "text/analytics/v2.0/keyPhrases";
                var response = await CallEndpoint(client, uri, byteData);
                result.Add(TextAnalyticsResultType.KeyPhrases, JsonConvert.DeserializeObject<TextAnalyticsResult>(response));

                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["numberOfLanguagesToDetect"] = noOfLanguages.ToString(CultureInfo.InvariantCulture);
                uri = "text/analytics/v2.0/languages?" + queryString;
                response = await CallEndpoint(client, uri, byteData);
                result.Add(TextAnalyticsResultType.Languages, JsonConvert.DeserializeObject<TextAnalyticsResult>(response));

                uri = "text/analytics/v2.0/sentiment";
                response = await CallEndpoint(client, uri, byteData);
                result.Add(TextAnalyticsResultType.Sentiment, JsonConvert.DeserializeObject<TextAnalyticsResult>(response));

                return result;
            }
        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
