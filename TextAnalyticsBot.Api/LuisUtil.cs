using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TextAnalyticsBot.DataModel.Luis;

namespace TextAnalyticsBot.Api
{
    public class LuisUtil
    {
        public static async Task<LuisQueryData> GetEntityFromLuis(string query)
        {
            query = Uri.EscapeDataString(query);
            LuisQueryData luisQueryData = new LuisQueryData();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=9867892d-d767-4795-b15c-fe07d1f35b3d&subscription-key=cffe9ea7a55c496c8086307fcc2a2408&q=" + query;
                HttpResponseMessage response = await client.GetAsync(RequestURI);

                if (response.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await response.Content.ReadAsStringAsync();
                    luisQueryData = JsonConvert.DeserializeObject<LuisQueryData>(JsonDataResponse);
                }
            }
            return luisQueryData;
        }
    }
}
