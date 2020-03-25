using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class SessionDb
    {
        public Guid Id;        
        [Column(TypeName = "jsonb")] 
        public List<Player> Players;
        public SessionState State;
    }
}