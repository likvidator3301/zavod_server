using System;

namespace Models
{
    public class Player
    {
        public ServerUserDto User { get; set; }
        public DateTimeOffset LastTimeActivity { get; set; }
    }
}