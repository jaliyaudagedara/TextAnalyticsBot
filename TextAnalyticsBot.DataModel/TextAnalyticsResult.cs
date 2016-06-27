using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel
{
    public class TextAnalyticsResult
    {
        public List<TextAnalyticsDocument> Documents { get; set; }
        public List<TextAnalyticsError> Errors { get; set; }
    }
}
