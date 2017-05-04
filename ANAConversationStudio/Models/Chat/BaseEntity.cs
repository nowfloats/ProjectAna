using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ANAConversationStudio.Models
{
    public class BaseIdEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string __id;
        public string _id
        {
            get { return __id; }
            set
            {
                if (__id != value)
                {
                    __id = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class BaseIdTimeStampEntity : BaseIdEntity
    {
        private DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if (_CreatedOn != value)
                {
                    _CreatedOn = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get { return _UpdatedOn; }
            set
            {
                if (_UpdatedOn != value)
                {
                    _UpdatedOn = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class BaseEntity : BaseIdEntity
    {
        private string _Tag;
        [Category("Important")]
        public string Tag
        {
            get { return _Tag; }
            set
            {
                if (_Tag != value)
                {
                    _Tag = value;
                    OnPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return Tag;
        }
    }

}