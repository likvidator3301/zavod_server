using System;
using System.Collections.Generic;

namespace Models
{
    public class ServerUserDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Email { get; set; }
        public PlayerDto MyPlayer { get; set; }
    }
}