using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Helpers
{
    public static class Utils
    {
        public static Settings Settings { get; set; }
        public static BasicAuth BasicAuth { get; set; }
    }
    public class BasicAuth
    {
        public string APIKey { get; set; }
        public string APISecret { get; set; }

        public string GetBase64()
        {
            if (string.IsNullOrWhiteSpace(APIKey) || string.IsNullOrWhiteSpace(APISecret))
                return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{APIKey}:{APISecret}"));
        }
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
