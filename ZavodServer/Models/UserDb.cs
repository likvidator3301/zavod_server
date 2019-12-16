using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ZavodServer.Models
{
    public class UserDb
    {
        public Guid Id { get; set; }
        [Key]
        public string Email { get; set; }
        public List<Guid> Units { get; set; }
        public List<Guid> Buildings { get; set; }
    }
}