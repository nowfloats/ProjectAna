using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace ANAConversationSimulator.UIHelpers
{
    public class ButtonTypeToInputScopeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ButtonTypeEnum buttonType)
            {
                switch (buttonType)
                {
                    case ButtonTypeEnum.GetPhoneNumber:
                    case ButtonTypeEnum.GetNumber:
                        {
                            var scope = new InputScope() { };
                            scope.Names.Add(new InputScopeName { NameValue = InputScopeNameValue.Number });
                            return scope;
                        }
                    case ButtonTypeEnum.GetEmail:
                        {
                            var scope = new InputScope() { };
                            scope.Names.Add(new InputScopeName { NameValue = InputScopeNameValue.EmailNameOrAddress });
                            return scope;
                        }
                    //case ButtonTypeEnum.PostText:
                    //case ButtonTypeEnum.GetText:
                    //    return InputScopeNameValue.Text;
                    default:
                        {
                            var scope = new InputScope() { };
                            scope.Names.Add(new InputScopeName { NameValue = InputScopeNameValue.Text });
                            return scope;
                        }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
