using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel
{
    public class TextAnalyticsMessage
    {
        public TextAnalyticsMessage()
        {
            Documents = new List<TextAnalyticsDocument>();
        }

        public List<TextAnalyticsDocument> Documents { get; set; }
    }
}
