using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace ZavodServer.Models
{
    public class BagDb
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public int GoldCount { get; set; }
        [Column(TypeName = "jsonb")]
        public Vector3 Position { get; set; }
    }
}