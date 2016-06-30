using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisQueryData
    {
        public string Query { get; set; }
        public List<LuisIntent> Intents { get; set; }
        public List<LuisEntity> Entities { get; set; }
        public LuisIntent TopScoringIntent { get; set; }
        public LuisDialogPrompt Dialog { get; set; }
    }
}
