using System;

namespace Models
{
    public class EnterSessionRequest
    {
        public Guid SessionId { get; set; }
        public string Nickname { get; set; }
    }
}