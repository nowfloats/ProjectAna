using ANAConversationStudio.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

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
        [JsonIgnore]
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

        protected override void FillAlias()
        {
            Alias = Utilities.TrimText(string.IsNullOrWhiteSpace(_Text) ? SectionType + "" : _Text);
        }
    }
}