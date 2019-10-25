using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [ApiController]
    [Route("objects/")]
    public class ObjectController : ControllerBase
    {
        private DatabaseContext db = new DatabaseContext();

        [HttpGet("all")]
        public List<Guid> GetObjectsId() => db.Objects.Select(x => x.Id).ToList();

        [HttpGet("{id}")]
        public ObjectDto GetObjectById([FromRoute] Guid id) => db.Objects.First(x => x.Id == id);

        [HttpPost("save")]
        public string CreateObject(ObjectDto objectDto)
        {
            db.Objects.Add(objectDto);
            db.SaveChanges();
            return "Ok";
        }
    }
}