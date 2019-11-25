using System;

namespace Models
{
    public class MoveUnitDto
    {
        public Guid Id { get; set; }
        public Vector3 NewPosition { get; set; }
    }
}