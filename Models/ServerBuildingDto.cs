using System;
using System.Numerics;

namespace Models
{
    public class ServerBuildingDto
    {
        public Guid Id { get; set; }

        public BuildingType Type { get; set; }

        public Vector3 Position { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Position: x: {Position.X} y: {Position.Y} z: {Position.Z}, Type: {Type}";
        }
    }

    public class DefaultSeverBuildDto
    {
         public BuildingType Type { get; set; }

        public ServerBuildingDto BuildingDto { get; set; }
    }
}