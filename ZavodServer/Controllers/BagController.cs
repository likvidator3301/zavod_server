using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    public class BagController : BaseController
    {
        public BagController(DatabaseContext db) : base(db) { }

        /// <summary>
        ///     Возвращает все мешки в сесиии 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<BagDb>> GetAll()
        {
            if (Session == null)
                return BadRequest();
            return Ok(Db.Bags.Where(x => x.SessionId.Equals(Session.Id)));
        }
        
        /// <summary>
        ///     Удаляет мешок
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<BagDb>>> Destroy([FromRoute]Guid id)
        {
            if (Session == null)
                return BadRequest();
            var bag = await Db.Bags.FirstOrDefaultAsync(x => x.Id.Equals(id) && x.SessionId.Equals(Session.Id));
            if (bag == null)
                return NotFound();
            Db.Bags.Remove(bag);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bagDb"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBag([FromBody] BagDb bagDb)
        {
            if (Session == null)
                return BadRequest();
            bagDb.SessionId = Session.Id;
            Db.Bags.Add(bagDb);
            return Ok();
        }
    }
}