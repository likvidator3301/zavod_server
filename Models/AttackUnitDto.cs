using System;

namespace Models
{
    public class AttackUnitDto
    {
        public Guid AttackUnitId { get; set; }
        public Guid DefenceUnitId { get; set; }
        public int Damage { get; set; }
    }
}