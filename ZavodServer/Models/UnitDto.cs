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
        public float AttackDamage { get; set; }
        public float AttackDelay { get; set; }
        public float AttackRange { get; set; }
        public float Defense { get; set; }
        public float MoveSpeed { get; set; }
        public float MaxHp { get; set; }
        public float CurrentHp { get; set; }
        public override string ToString()
        {
            return
                $"Id: {Id}, Position: x: {Position.X} y: {Position.Y} z: {Position.Z}, " +
                $"Rotation: x: {Rotation.X} y: {Rotation.Y} z: {Rotation.Z}, Type: {Type}";
        }

        public void Copy(UnitDto unitDto)
        {
            Id = unitDto.Id;
            Position = unitDto.Position;
            Rotation = unitDto.Rotation;
            Type = unitDto.Type;
            AttackDamage = unitDto.AttackDamage;
            AttackDelay = unitDto.AttackDelay;
            AttackRange = unitDto.AttackRange;
            Defense = unitDto.Defense;
            MoveSpeed = unitDto.MoveSpeed;
            MaxHp = unitDto.MaxHp;
            CurrentHp = unitDto.CurrentHp;
        } 
    }
    [ComplexType]
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
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