using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class UnitDb
    {
        [Key] 
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid SessionId { get; set; }
        public int Health { get; set; }
        [Column(TypeName = "jsonb")]
        public Vector3 Position { get; set; }
        [Column(TypeName = "jsonb")]
        public Vector3 RotationInEulerAngle { get; set; }
        public UnitType Type { get; set; }
        [Column(TypeName = "jsonb")]
        public Dictionary<string, string> Requisites { get; set; }

        public override string ToString()
        {
            return
                $"Id: {Id}, Position: x: {Position.X} y: {Position.Y} z: {Position.Z}, " +
                $"Rotation: x: {RotationInEulerAngle.X} y: {RotationInEulerAngle.Y} z: {RotationInEulerAngle.Z}, Type: {Type}, " +
                $"OwnerId: {PlayerId}, SessionId: {SessionId}";

        }

        public void Copy(UnitDb unitDto)
        {
            Id = unitDto.Id;
            Position = unitDto.Position;
            RotationInEulerAngle = unitDto.RotationInEulerAngle;
            Type = unitDto.Type;
            Health = unitDto.Health;
            PlayerId = unitDto.PlayerId;
            SessionId = unitDto.SessionId;
            Requisites = unitDto.Requisites;
        }

        public static int GetMaxHpFromType(UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Base:
                    return DefaultUnitHp.Base;
                case UnitType.Warrior:
                    return DefaultUnitHp.Warrior;
                case UnitType.Runner:
                    return DefaultUnitHp.Runner;
                case UnitType.Garage:
                    return DefaultUnitHp.Garage;
                case UnitType.Stall:
                    return DefaultUnitHp.Stall;
                case UnitType.Tower:
                    return DefaultUnitHp.Tower;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }
        
        private class DefaultUnitHp
        {
            public const int Warrior = 100;
            public const int Runner  = 60;
            public const int Garage = 200;
            public const int Stall  = 200;
            public const int Tower = 150;
            public const int Base = 350;
        }
    }
}