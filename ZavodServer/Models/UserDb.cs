using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Models;

namespace ZavodServer.Models
{
    public class UserDb
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        [Key]
        public string Email { get; set; }
        [Column(TypeName = "jsonb")] 
        public List<Currency> Currencies { get; set; }
    }
}