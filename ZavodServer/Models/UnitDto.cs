using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZavodServer.Models
{
    public class UnitDto
    {
        [Key]
        public Guid Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public string Type { get; set; }
        
        public override string ToString()
        {
            return string.Format("Id: {0}, Postion: x: {1} y: {2} z: {3}, Rotation: x: {4} y: {5} z: {6}, Type: {7}", 
                Id, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Type);
        }
    }
    [ComplexType]
    public class Vector3
    {
        [Column("X")]
        public float X { get; set; }
        [Column("Y")]
        public float Y { get; set; }
        [Column("Z")]
        public float Z { get; set; }

        public static float Distance(Vector3 first, Vector3 second)
        {
            float num1 = first.X - second.X;
            float num2 = first.Y - second.Y;
            float num3 = first.Z - second.Z;
            return MathF.Sqrt((float) ((double) num1 * (double) num1 + (double) num2 * (double) num2 + (double) num3 * (double) num3));
        }
    }
}