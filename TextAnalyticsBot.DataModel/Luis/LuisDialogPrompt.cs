using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisDialogPrompt
    {
        public string Prompt { get; set; }
        public string ParameterName { get; set; }
        public string ContextId { get; set; }
        public string Status { get; set; }
    }
}
