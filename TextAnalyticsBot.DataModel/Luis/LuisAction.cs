using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisAction
    {
        public bool Triggered { get; set; }
        public string Name { get; set; }
        public List<LuisParameters> Parameters { get; set; }
    }
}
