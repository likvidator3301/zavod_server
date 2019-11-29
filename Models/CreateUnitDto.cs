using System.Numerics;

namespace Models
{
    public class CreateUnitDto
    {
        public UnitType UnitType { get; set; }
        public Vector3 Position { get; set; }
    }
}