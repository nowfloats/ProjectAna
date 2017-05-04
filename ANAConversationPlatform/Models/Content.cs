using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Models
{
    public class Content
    {
        public string _id { get; set; }
        public string NodeName { get; set; }
        public string NodeId { get; set; }
        public string Emotion { get; set; }
        public string NodeHeaderText { get; set; }
        public string SectionId { get; set; }
        public string SectionNumber { get; set; }
        public string SectionText { get; set; }
        public string ButtonId { get; set; }
        public int Button { get; set; }
        public string ButtonName { get; set; }
        public string ButtonText { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string AltText { get; set; }

        public string XLabel { get; set; }
        public string YLabel { get; set; }
        public string CoordinateListLegend { get; set; }
        public string CoordinateListId { get; set; }
        public string CoordinateText { get; set; }

    }
}
