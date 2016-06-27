using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel
{
    public class TextAnalyticsResult
    {
        public List<TextAnalyticsDocument> Documents { get; set; }
        public List<TextAnalyticsError> Errors { get; set; }
    }
}
