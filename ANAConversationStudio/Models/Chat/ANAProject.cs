using ANAConversationStudio.Helpers;

namespace ANAConversationStudio.Models.Chat
{
    public class ANAProject : BaseIdEntity
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool IsValid()
        {
            return Utilities.ValidateStrings(Name);
        }
    }
}
