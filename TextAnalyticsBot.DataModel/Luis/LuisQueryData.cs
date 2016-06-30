using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
