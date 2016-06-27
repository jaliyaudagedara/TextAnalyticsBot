using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel
{
    public class TextAnalyticsDocument
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public List<string> KeyPhrases { get; set; }
        public List<TextAnalyticsLanguage> DetectedLanguages { get; set; }
        public double Score { get; set; }
    }
}