using System;
using System.Collections.Generic;

namespace Models
{
    public class ServerUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public List<Guid> Units { get; set; }
        public List<Guid> Buildings { get; set; }
    }
}