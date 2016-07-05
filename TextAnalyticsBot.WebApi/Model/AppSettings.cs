using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextAnalyticsBot.WebApi.Model
{
    public class AppSettings
    {
        public string BaseUrl { get; set; }
        public string AccountKey { get; set; }
        public int NumLanguages { get; set; }
    }
}
