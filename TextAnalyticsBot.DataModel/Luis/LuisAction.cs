using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisAction
    {
        public bool Triggered { get; set; }
        public string Name { get; set; }
        public List<LuisParameters> Parameters { get; set; }
    }
}
