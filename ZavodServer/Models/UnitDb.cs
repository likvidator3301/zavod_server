using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class UnitDb
    {
        [Key] 
        public Guid Id { get; set; }
        // public Guid SessionId { get; set; }
        [Column(TypeName = "jsonb")] 
        public Vector3 Position { get; set; }
        [Column(TypeName = "jsonb")] 
        public Vector3 Rotation { get; set; }
        public UnitType Type { get; set; }
        public float AttackDamage { get; set; }
        public float AttackDelay { get; set; }
        public float AttackRange { get; set; }
        public float Defense { get; set; }
        public float MoveSpeed { get; set; }
        public float MaxHp { get; set; }
        public float CurrentHp { get; set; }
        public float LastAttackTime { get; set; }

        public override string ToString()
        {
            return
                $"Id: {Id}, Position: x: {Position.X} y: {Position.Y} z: {Position.Z}, " +
                $"Rotation: x: {Rotation.X} y: {Rotation.Y} z: {Rotation.Z}, Type: {Type}, " +
                $"AttackDamage: {AttackDamage}, AttackDelay: {AttackDelay}, AttackRange: {AttackRange}, " +
                $"Defense: {Defense}, MoveSpeed: {MoveSpeed}, MaxHp: {MaxHp}, CurrentHp: {CurrentHp}, " +
                $"LastAttackTime: {LastAttackTime}";
        }

        public void Copy(UnitDb unitDto)
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
            LastAttackTime = unitDto.LastAttackTime;
        }
    }

    public class DefaultUnitDb
    {
        [Key] 
        public UnitType Type { get; set; }
        [Column(TypeName = "jsonb")] 
        public UnitDb UnitDto { get; set; }
    }
}