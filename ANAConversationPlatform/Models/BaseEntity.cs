using System;

namespace ANAConversationPlatform.Models
{
    public class BaseIdEntity
    {
        public string _id { get; set; }
    }

    public class BaseIdTimestampEntity : BaseIdEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class BaseEntity : BaseIdTimestampEntity
    {
        public string Alias { get; set; }
    }
}