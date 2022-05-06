using System;

namespace Mensageira.Model
{
    public class MessageDTO
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
