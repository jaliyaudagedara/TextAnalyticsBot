using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisIntent
    {
        public string Intent { get; set; }
        public float Score { get; set; }
        public List<LuisAction> Actions { get; set; }
    }
}
