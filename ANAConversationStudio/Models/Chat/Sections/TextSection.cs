namespace ANAConversationStudio.Models.Chat.Sections
{
    public class TextSection : Section
    {
        public TextSection()
        {
            SectionType = SectionTypeEnum.Text;
            FillAlias();
        }

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

                    FillAlias();
                    OnPropertyChanged();
                }
            }
        }

        private void FillAlias()
        {
            Alias = string.IsNullOrWhiteSpace(_Text) ? SectionType + "" : _Text;
        }
    }
}