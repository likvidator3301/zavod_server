using System;
using System.Collections.Generic;

namespace Models
{
    public class OutputUnitState
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public int Health { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 RotationInEulerAngle { get; set; }
        public UnitType Type { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
    }
}