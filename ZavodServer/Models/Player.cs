using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZavodServer.Models
{
    public class Player
    {        
        [Column(TypeName = "jsonb")] 
        public UserDb User { get; set; }
        public DateTimeOffset LastTimeActivity { get; set; }
    }
}