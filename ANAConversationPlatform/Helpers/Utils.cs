using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Helpers
{
    public static class Utils
    {
        public static Settings Settings { get; set; }
    }

    public class Settings
    {
        public string StudioDistFolder { get; set; }
    }
}
