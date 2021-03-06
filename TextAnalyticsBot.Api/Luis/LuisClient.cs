﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TextAnalyticsBot.DataModel.Luis;

namespace TextAnalyticsBot.Api.Luis
{
    public class LuisClient
    {
        public static async Task<LuisQueryData> SendRequest(string strInput, string contextId = "")
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                string uri = string.Empty;
                if (string.IsNullOrEmpty(contextId))
                {
                    uri = "https://api.projectoxford.ai/luis/v1/application/preview?id=9867892d-d767-4795-b15c-fe07d1f35b3d&subscription-key=bf82958d57934b0a9b79d2eb6d2687a2&q=" + strEscaped;
                }
                else
                {
                    uri = "https://api.projectoxford.ai/luis/v1/application/preview?id=9867892d-d767-4795-b15c-fe07d1f35b3d&subscription-key=bf82958d57934b0a9b79d2eb6d2687a2&q=" + strEscaped + "&contextId=" + contextId;
                }
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    LuisQueryData luisQueryData = JsonConvert.DeserializeObject<LuisQueryData>(jsonResponse);
                    return luisQueryData;
                }
            }
            return null;
        }
    }
}
