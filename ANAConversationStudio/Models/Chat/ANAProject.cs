namespace ANAConversationStudio.Models.Chat
{
    public class ANAProject : BaseIdEntity
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
