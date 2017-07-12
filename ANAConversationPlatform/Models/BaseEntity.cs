using System;

namespace ANAConversationPlatform.Models
{
    public class BaseEntity
    {
        public string _id { get; set; }
    }

    public class BaseTimestampEntity : BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}