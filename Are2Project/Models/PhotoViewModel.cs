using System.Collections.Generic;

namespace Are2Project.Models
{
    public class PhotoDescriptionViewModel
        {
            public string title { get; set; }
            public string description { get; set; }
            public List<string> thumbnail { get; set; }
            public List<string> large { get; set; }
            public List<string> button_list { get; set; }
            public List<string> tags { get; set; }

        }
    }
