using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;

namespace TransportAnimals.Controllers
{
    [Route("animals/types")]
    [ApiController]
    public class AnimalTypeController : ControllerBase
    {
        private readonly ApplicationContext db;
        public AnimalTypeController(ApplicationContext db)
        {
            this.db = db;
        }

        [HttpGet("{typeId:long?}")]
        public ActionResult<ResponseAnimalType> GetAnimalTypeById([Required][GreaterEqual(1)] long? typeId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            AnimalType? t = db.AnimalTypes.FirstOrDefault(t => t.Id == typeId);
            return t != null ? Ok(new ResponseAnimalType(t)) : NotFound();
        }

        [HttpPost]
        public ActionResult<ResponseAnimalType> AddAnimalType([FromBody] RequestAnimalType requestAnimalType)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            AnimalType? t = db.AnimalTypes.FirstOrDefault(t => t.Type == requestAnimalType.Type);
            if (t != null)
                return Conflict();
            else
            {
                t = new AnimalType() { Type = requestAnimalType.Type };
                db.AnimalTypes.Add(t);
                db.SaveChanges();
                return CreatedAtRoute("", new ResponseAnimalType(t));
            }
        }

        [HttpPut("{typeId:long?}")]
        public ActionResult<ResponseAnimalType> UpdateAnimalTypeById([Required][GreaterEqual(1)] long? typeId, [FromBody] RequestAnimalType requestAnimalType)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            AnimalType? t = db.AnimalTypes.FirstOrDefault(t => t.Type == requestAnimalType.Type);
            if (t != null)
                return Conflict();
            t = db.AnimalTypes.FirstOrDefault(t => t.Id == typeId);
            if (t == null)
                return NotFound();
            t.Type = requestAnimalType.Type;
            db.SaveChanges();
            return Ok(new ResponseAnimalType(t));
        }

        [HttpDelete("{typeId:long?}")]
        public ActionResult DeleteLocationPointById([Required][GreaterEqual(1)] long? typeId)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            AnimalType? t = db.AnimalTypes.FirstOrDefault(t => t.Id == typeId.Value);
            if (t == null)
                return NotFound();
            if (db.Animals.Any(a => a.AnimalTypes.Any(t => t.Id == typeId.Value)))
                return BadRequest();
            db.AnimalTypes.Remove(t);
            db.SaveChanges();
            return Ok();
        }
    }
}
