using System;
using System.ComponentModel.DataAnnotations;

namespace ZavodServer.Models
{
    public class ObjectDto
    {
        [Key]
        public Guid Id { get; set; }
        public double[] Position { get; set; }
        public int Rotation { get; set; }
        public string Type { get; set; }
        
        public override string ToString()
        {
            return string.Format("Id: {0}, Postion: x: {1} y: {2} z: {3}, Rotation: {4}, Type: {5}", 
                Id, Position[0], Position[1], Position[2], Rotation, Type);
        }
    }
}