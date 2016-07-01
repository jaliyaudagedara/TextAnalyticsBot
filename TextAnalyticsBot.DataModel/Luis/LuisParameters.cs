using System.Collections.Generic;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisParameters
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public List<LuisParameterValue> Value { get; set; }
    }
}
