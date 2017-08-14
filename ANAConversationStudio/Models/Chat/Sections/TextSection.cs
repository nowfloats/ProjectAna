namespace ANAConversationStudio.Models.Chat.Sections
{
    public class TextSection : Section
    {
        public TextSection() { SectionType = SectionTypeEnum.Text; }

        //Content
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;

                    Alias = _Text;
                    OnPropertyChanged();
                }
            }
        }
    }
}