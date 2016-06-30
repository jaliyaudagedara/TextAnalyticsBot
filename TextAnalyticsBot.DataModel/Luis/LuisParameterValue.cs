using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisParameterValue
    {
        public string Entity { get; set; }
        public string Type { get; set; }
        public float Score { get; set; }
    }
}
