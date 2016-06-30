using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisParameters
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public List<LuisParameterValue> Value { get; set; }
    }
}
