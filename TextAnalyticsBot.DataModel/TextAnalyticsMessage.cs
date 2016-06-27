using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel
{
    public class TextAnalyticsMessage
    {
        public TextAnalyticsMessage()
        {
            Documents = new List<TextAnalyticsDocument>();
        }

        public List<TextAnalyticsDocument> Documents { get; set; }
    }
}
