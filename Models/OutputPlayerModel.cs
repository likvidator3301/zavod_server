using System;
using System.Collections.Generic;

namespace Models
{
    public class OutputPlayerModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public PlayerResources Resources { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
    }
}