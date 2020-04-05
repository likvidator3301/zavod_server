using System;
using System.Collections.Generic;

namespace Models
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public List<PlayerDto> Players { get; set; }
        public MapDto GameMap { get; set; }
        public SessionState State { get; set; }
    }
}