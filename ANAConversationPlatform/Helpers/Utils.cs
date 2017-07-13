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
        public int MaxCapTimeTakenToType { get; set; }
        public int BaseTimeTakenToTypePerChar { get; set; }
        public int VariableTimeTakenToTypePerChar { get; set; }
    }
    public class APIResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
    public class DataResponse<T> : APIResponse
    {
        public T Data { get; set; }
    }
}
