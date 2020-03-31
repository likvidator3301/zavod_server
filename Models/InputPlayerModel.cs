using System.Collections.Generic;

namespace Models
{
    public class InputPlayerModel
    {
        public PlayerResources Resources { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
    }
}