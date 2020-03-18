using System;
using System.Collections.Generic;

namespace ZavodServer.Models
{
    public class SessionDb
    {
        public Guid Id;
        public List<Guid> UserIds;
        public int State;
    }
}