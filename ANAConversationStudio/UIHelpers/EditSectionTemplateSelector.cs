using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ANAConversationStudio.UIHelpers
{
    public class EditSectionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var templateName = "";
            var section = item as Section;
            switch (section.SectionType)
            {
                case SectionTypeEnum.Image:
                case SectionTypeEnum.Gif:
                case SectionTypeEnum.Audio:
                case SectionTypeEnum.Video:
                case SectionTypeEnum.EmbeddedHtml:
                    templateName = "TitleCaptionUrlSectionEditTemplate";
                    break;
                case SectionTypeEnum.Text:
                    templateName = "TextSectionEditTemplate";
                    break;
                case SectionTypeEnum.Carousel:
                    templateName = "CarouselSectionEditTemplate";
                    break;
                case SectionTypeEnum.PrintOTP:
                    templateName = "TextSectionEditTemplate";
                    //Not supported yet.
                    break;
                default:
                    break;
            }
            return Application.Current.Resources[templateName] as DataTemplate;
        }
    }
}
