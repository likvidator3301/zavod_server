using System;
using System.Collections.Generic;
using System.Text;

namespace ZavodClient
{
    public class UnitDto
    {
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
        public float LastAttackTime { get; set; }

        public UnitDto()
        {
            Id = new Guid();
            Position = new Vector3();
            Rotation = new Vector3();
            Type = "Unit";
            AttackDamage = 120;
            AttackDelay = 100;
            AttackRange = 23;
            Defense = 40;
            MoveSpeed = 20;
            MaxHp = 100;
            CurrentHp = 70;
            LastAttackTime = 3;
        }

    }
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3()
        {
            X = 5;
            Y = 10.5f;
            Z = 2f;
        }
    }
}
