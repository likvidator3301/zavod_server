using System;
using System.Collections.Generic;

namespace Models
{
    public class InputUnitState
    {
        public Guid Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 RotationInEulerAngle { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
        public UnitType Type { get; set; }
    }
}