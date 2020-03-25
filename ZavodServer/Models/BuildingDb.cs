using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class BuildingDb
    {
        [Key] 
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid SessionId { get; set; }
        [Column(TypeName = "jsonb")] 
        public Currency Cost { get; set; }
        public BuildingType Type { get; set; }
        [Column(TypeName = "jsonb")] 
        public Vector3 Position { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Position: x: {Position.X} y: {Position.Y} z: {Position.Z}, Type: {Type}, " +
                   $"Cost: {Cost}, OwnerId: {OwnerId}, SessionId: {SessionId}";
        }
    }

    public class DefaultBuildingDb
    {
        [Key] 
        public BuildingType Type { get; set; }
        [Column(TypeName = "jsonb")] 
        public BuildingDb BuildingDto { get; set; }
    }
}