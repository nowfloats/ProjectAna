namespace ANAConversationPlatform.Models.Sections
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
