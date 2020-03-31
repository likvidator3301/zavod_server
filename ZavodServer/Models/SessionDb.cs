using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class SessionDb
    {
        [Key]
        public Guid Id { get; set; }
        [Column(TypeName = "jsonb")] 
        public List<Player> Players { get; set; }
        [Column(TypeName = "jsonb")] 
        public Map GameMap { get; set; }
        public SessionState State { get; set; }
    }
}