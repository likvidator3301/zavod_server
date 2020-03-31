using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class Player
    {        
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        [Column(TypeName = "jsonb")]
        public PlayerResources Resources { get; set; }
        [Column(TypeName = "jsonb")]
        public Dictionary<string, string> Requisites { get; set; }
        public DateTimeOffset LastTimeActivity { get; set; }
    }
}