using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZavodServer.Models
{
    public class UserDb
    {
        public Guid Id;
        public string Email;
        public List<Guid> Units;
        public List<Guid> Buildings;
    }
}