using System;

namespace Models
{
    public class BagDto
    {
        public Guid Id { get; set; }
        public int GoldCount { get; set; }
        public Vector3 Position { get; set; }
    }
}