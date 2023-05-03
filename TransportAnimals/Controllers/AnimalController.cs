using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;

namespace TransportAnimals.Controllers
{
    [Route("animals")]
	[ApiController]
	public class AnimalController : ControllerBase
	{
		private readonly ApplicationContext db;
		public AnimalController(ApplicationContext db)
		{
			this.db = db;
		}

		[HttpGet("{animalId:long?}")]
		public ActionResult<ResponseAnimal> GetAnimalById([Required][GreaterEqual(1)] long? animalId)
		{
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Animal? a = db.Animals.Include(a => a.VisitedLocations).Include(a => a.AnimalTypes).FirstOrDefault(a => a.Id == animalId);
			return a != null ? Ok(new ResponseAnimal(a)) : NotFound();
		}

		[HttpGet("search")]
		public ActionResult<IEnumerable<ResponseAnimal>> SearchAnimal(
			[FromQuery] DateTimeOffset? startDateTime,
			[FromQuery] DateTimeOffset? endDateTime,
			[FromQuery][GreaterEqual(1)] int? chipperId,
			[FromQuery][GreaterEqual(1)] long? chippingLocationId,
			[FromQuery][EnumDataType(typeof(Helpers.LifeStatus))] string? lifeStatus,
			[FromQuery][EnumDataType(typeof(Helpers.Gender))] string? gender,
			[FromQuery][GreaterEqual(0)] int from = 0,
			[FromQuery][GreaterEqual(1)] int size = 10)
		{
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            var selectedAnimals = (from a in db.Animals.Include(t => t.AnimalTypes).Include(t => t.VisitedLocations)
								   where startDateTime == null || a.ChippingDateTime > startDateTime
								   where endDateTime == null || a.ChippingDateTime < endDateTime
								   where chipperId == null || a.ChipperId == chipperId
								   where chippingLocationId == null || a.ChippingLocationId == chippingLocationId
								   where lifeStatus == null || a.LifeStatus == lifeStatus
								   where gender == null || a.Gender == gender
								   orderby a.Id
								   select a).Skip(from).Take(size);
			return Ok(Array.ConvertAll(selectedAnimals.ToArray(), a => new ResponseAnimal(a)));
		}

		[HttpPost]
		public ActionResult<ResponseAnimal> AddAnimal([FromBody] RequestAddAnimal animal)
		{
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            if (animal.AnimalTypes.AsParallel().Any(a => a <= 0))
				return BadRequest();
			bool isUnique = animal.AnimalTypes.GroupBy(x => x).All(x => x.Count() == 1);
			if (!isUnique)
				return Conflict();

			Account? acc = db.Accounts.FirstOrDefault(acc => acc.Id == animal.ChipperId);
			LocationPoint? p = db.Locations.FirstOrDefault(p => p.Id == animal.ChippingLocationId);
			bool con = animal.AnimalTypes.All(x => db.AnimalTypes.Select(ant => ant.Id).Contains(x));
			if (acc == null || !con || p == null)
				return NotFound();
			DateTimeOffset dateTime = DateTimeOffset.Now;
			Animal a = new ()
			{
				AnimalTypes = new List<AnimalType>(),
				Weight = animal.Weight.Value,
				Length = animal.Length.Value,
				Height = animal.Height.Value,
				Gender = animal.Gender,
				ChipperId = animal.ChipperId.Value,
				ChippingLocationId = animal.ChippingLocationId.Value,
				DeathDateTime = null,
				ChippingDateTime = dateTime,
				LifeStatus = "ALIVE",
				VisitedLocations = new List<AnimalVisitedLocation>()
			};
			for (int i = 0; i < animal.AnimalTypes.Length; i++)
			{
				a.AnimalTypes.Add(db.AnimalTypes.FirstOrDefault(at => at.Id == animal.AnimalTypes[i]));
			}
            db.Animals.Add(a);
            db.SaveChanges();
			return CreatedAtRoute("", new ResponseAnimal(a));
		}

		[HttpPut("{animalId:long?}")]
		public ActionResult<ResponseAnimal> UpdateAnimalById([Required][GreaterEqual(1)] long? animalId, [FromBody] RequestUpdateAnimal animal)
		{
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? a = db.Animals.Include(a => a.VisitedLocations).Include(a => a.AnimalTypes).FirstOrDefault(a => a.Id == animalId);
			if (a != null)
			{
				if (a.LifeStatus == "DEAD" && animal.LifeStatus == "ALIVE" || (a.VisitedLocations.Count >= 1 && animal.ChippingLocationId == a.VisitedLocations[0].LocationPointId))
                    return BadRequest();
				if (!db.Accounts.Any(a => a.Id == animal.ChipperId) || !db.Locations.Any(l => l.Id == animal.ChippingLocationId))
					return NotFound();
                DateTimeOffset dateTime = DateTimeOffset.Now;
				a.LifeStatus = animal.LifeStatus;
				a.Length = animal.Length.Value;
				a.Weight = animal.Weight.Value;
				a.Height = animal.Height.Value;
				a.Gender = animal.Gender;
				a.ChipperId = animal.ChipperId.Value;
				a.ChippingLocationId = animal.ChippingLocationId.Value;
				a.DeathDateTime = a.LifeStatus == "DEAD" ? dateTime : null;
				db.SaveChanges();
				return Ok(new ResponseAnimal(a));
			}
			else
				return NotFound();
		}

		[HttpDelete("{animalId:long?}")]
		public ActionResult DeleteAnimalById([Required][GreaterEqual(1)] long? animalId)
		{
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? a = db.Animals.Include(a => a.AnimalTypes).Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId);
			if (a != null)
			{
				if (a.VisitedLocations.Count >= 1)
					return BadRequest();
				db.Animals.Remove(a);
				db.SaveChanges();
				return Ok();
			}
			else
				return NotFound();
		}

		[HttpPost("{animalId:long?}/types/{typeId:long?}")]
		public ActionResult<ResponseAnimal> AddAnimalTypeForAnimal([Required][GreaterEqual(1)] long? animalId, [Required][GreaterEqual(1)] long? typeId)
		{
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? an = db.Animals.Include(a => a.AnimalTypes).Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId.Value);
			AnimalType? at = db.AnimalTypes.FirstOrDefault(p => p.Id == typeId.Value);
			if (an == null || at == null)
				return NotFound();
			AnimalType? animType = an.AnimalTypes.FirstOrDefault(ant => ant.Id == typeId.Value);
			if (animType != null)
				return Conflict();
			an.AnimalTypes.Add(at);
			db.SaveChanges();
			return CreatedAtRoute("", new ResponseAnimal(an));
		}

		[HttpPut("{animalId:long?}/types")]
		public ActionResult<ResponseAnimal> ChangeAnimalTypeForAnimal([Required][GreaterEqual(1)] long? animalId, [FromBody] RequestChangeAnimalType requestChangeAnimalType)
		{
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? an = db.Animals.Include(a => a.AnimalTypes).Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId.Value);
			AnimalType? at1 = db.AnimalTypes.FirstOrDefault(p => p.Id == requestChangeAnimalType.OldTypeId.Value);
			AnimalType? at2 = db.AnimalTypes.FirstOrDefault(p => p.Id == requestChangeAnimalType.NewTypeId.Value);
			if (an == null || at1 == null)
				return NotFound();
			AnimalType? at = an.AnimalTypes.FirstOrDefault(a => a.Id == at1.Id);
			if (at2 == null || at == null)
				return NotFound();
			if (an.AnimalTypes.FirstOrDefault(p => p.Id == at2.Id) != null)
				return Conflict();
			an.AnimalTypes.Remove(at);
			an.AnimalTypes.Add(at2);
			db.SaveChanges();
			return Ok(new ResponseAnimal(an));
		}

        [HttpDelete("{animalId:long?}/types/{typeId:long?}")]
        public ActionResult<ResponseAnimal> DeleteAnimalTypeForAnimal([Required][GreaterEqual(1)] long? animalId, [Required][GreaterEqual(1)] long? typeId) {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? an = db.Animals.Include(a => a.AnimalTypes).Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId.Value);
            AnimalType? at1 = db.AnimalTypes.FirstOrDefault(p => p.Id == typeId.Value);
            if (an == null || at1 == null)
                return NotFound();
            AnimalType? at = an.AnimalTypes.FirstOrDefault(a => a.Id == typeId.Value);
            if (at == null)
                return NotFound();
			if (an.AnimalTypes.Count == 1)
				return BadRequest();
            an.AnimalTypes.Remove(at);
            db.SaveChanges();
            return Ok(new ResponseAnimal(an));
        }
    }
}
