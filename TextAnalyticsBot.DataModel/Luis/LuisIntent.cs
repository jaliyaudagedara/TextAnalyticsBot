using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisIntent
    {
        public string Intent { get; set; }
        public float Score { get; set; }
    }
}
