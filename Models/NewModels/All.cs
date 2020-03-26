using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace NewModels1
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

    public class InputUnitState
    {
        public Guid Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 RotationInEulerAngle { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
        public UnitType Type { get; set; }
    }


    public enum UnitType
    {
        Warrior = 100,
        Runner = 101,
        Garage = 200,
        Stall = 201,
        Tower = 202,
        Base = 300
    }

    public class Vector3
    {

    }

    public class OutputPlayerModel
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public PlayerResources Resources { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
    }

    public class InputPlayerModel
    {
        public PlayerResources Resources { get; set; }
        public Dictionary<string, string> Requisites { get; set; }
    }

    public class EnterSessionRequest
    {
        public Guid SessionId { get; set; }
        public string Nickname { get; set; }
    }

    public class PlayerResources
    {
        public int Money { get; set; }
        public int Beer { get; set; }
    }
}