using ANAConversationSimulator.Models.Chat;

namespace ANAConversationSimulator.Models.Chat.Sections
{
    public class PrintOTPSection : Section
    {
        public int Length { get; set; }
        public PrintOTPSection()
        {
            SectionType = SectionTypeEnum.PrintOTP;
        }

        //Below fields will be filled at runtime
        public static string OTP { get; set; }
    }
}
