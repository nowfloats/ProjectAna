using ANAConversationSimulator.Models.Chat;
using ANAConversationSimulator.Models.Chat.Sections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ANAConversationSimulator.Helpers
{
    public static class VerbProcessor
    {
        const string VERB_REGEX = @"\[~(.*?)\]";
        public static string Process(string input, bool escape = true)
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
                    {
                        if (escape)
                            input = input.Replace(match.Value, JsonConvert.SerializeObject(value + "").Trim('"'));
                        else
                            input = input.Replace(match.Value, value + "");
                    }
                }
            }
            return input;
        }

        //public static string ProcessUnescaped(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input)) return "";
        //    var matches = Regex.Matches(input, VERB_REGEX);
        //    foreach (Match match in matches)
        //    {
        //        if (match.Groups.Count >= 2)
        //        {
        //            var varName = match.Groups[1].Value;
        //            var value = ButtonActionHelper.GetSavedValue(varName);
        //            if (!string.IsNullOrWhiteSpace(value + ""))
        //                input = input.Replace(match.Value, value + "");
        //        }
        //    }
        //    return input;
        //}

        public static List<CarouselButton> ProcessCarousalButtons(List<CarouselButton> sourceButtons, ChatNode parsedNode)
        {
            var resultBtns = new List<CarouselButton>();
            foreach (var btn in sourceButtons)
            {
                if (btn != null)
                {
                    if (btn.DoesRepeat)
                    {
                        var repeatOn = ButtonActionHelper.GetSavedArray(btn.RepeatOn);
                        var max = btn.MaxRepeats == 0 ? repeatOn.Count - 1 : btn.MaxRepeats;
                        for (int i = btn.StartPosition; i <= max; i++)
                        {
                            var b = btn.DeepCopy();
                            ButtonActionHelper.ClearSavedValue(b.RepeatAs);
                            ButtonActionHelper.HandleSaveTextInput(b.RepeatAs, repeatOn[i] + "");
                            b.Text = Process(b.Text);
                            b.Url = Process(b.Url);
                            b.VariableName = parsedNode.VariableName;
                            b.NodeId = parsedNode.Id;
                            ButtonActionHelper.ClearSavedValue(b.RepeatAs);
                            resultBtns.Add(b);
                        }
                    }
                    else
                    {
                        btn.VariableName = parsedNode.VariableName;
                        btn.NodeId = parsedNode.Id;
                        resultBtns.Add(btn);
                    }
                }
            }
            return resultBtns;
        }

        public static List<CarouselItem> ProcessCarousalItems(List<CarouselItem> items, ChatNode parsedNode)
        {
            var resultItems = new List<CarouselItem>();
            foreach (var item in items)
            {
                if (item.DoesRepeat)
                {
                    var repeatOn = ButtonActionHelper.GetSavedArray(item.RepeatOn);
                    var max = item.MaxRepeats == 0 ? repeatOn.Count - 1 : item.MaxRepeats;
                    for (int i = item.StartPosition; i <= max; i++)
                    {
                        var it = item.DeepCopy();

                        ButtonActionHelper.ClearSavedValue(it.RepeatAs);
                        ButtonActionHelper.HandleSaveTextInput(it.RepeatAs, repeatOn[i] + "");
                        it.Title = VerbProcessor.Process(it.Title);
                        it.Caption = VerbProcessor.Process(it.Caption);
                        it.ImageUrl = VerbProcessor.Process(it.ImageUrl);
                        it.Buttons = VerbProcessor.ProcessCarousalButtons(it.Buttons, parsedNode);
                        ButtonActionHelper.ClearSavedValue(it.RepeatAs);
                    }
                    resultItems.Add(item);
                }
                else
                {
                    item.Buttons = VerbProcessor.ProcessCarousalButtons(item.Buttons, parsedNode);
                    resultItems.Add(item);
                }
            }
            return resultItems;
        }
    }
}
