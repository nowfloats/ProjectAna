using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ANAConversationSimulator.Helpers
{
    public static class VerbProcessor
    {
        const string VERB_REGEX = @"\[~(.*?)\]";
        public static string Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            var matches = Regex.Matches(input, VERB_REGEX);
            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 2)
                {
                    var varName = match.Groups[1].Value;
                    var value = ButtonActionHelper.GetSavedValue(varName);
                    if (!string.IsNullOrWhiteSpace(value + ""))
                        input = input.Replace(match.Value, JsonConvert.SerializeObject(value + "").Trim('"'));
                }
            }
            return input;
        }
    }
}
