using System;

namespace TextAnalyticsBot.DataModel.Feedback
{
    [Serializable]
    public class Feedback
    {
        public string Country { get; set; }
        public string Event { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsSubmitted { get; set; }
    }
}
