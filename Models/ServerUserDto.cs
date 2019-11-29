using System;
using System.Collections.Generic;

namespace Models
{
    public class ServerUserDto
    {
        public Guid Id;
        public string Email;
        public List<ServerUnitDto> Units;
        public List<ServerBuildingDto> Buildings;
    }
}