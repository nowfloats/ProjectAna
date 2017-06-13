using ANAConversationStudio.Models.Chat;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class PrintOTPSection : Section
    {
        public int Length { get; set; }
        public PrintOTPSection()
        {
            SectionType = SectionTypeEnum.PrintOTP;
        }
    }
}
