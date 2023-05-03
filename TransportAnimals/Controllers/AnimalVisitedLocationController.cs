using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;
using TransportAnimals.Helpers;

namespace TransportAnimals.Controllers
{
    [Route("animals")]
    [ApiController]
    public class AnimalVisitedLocationController : ControllerBase
    {
        private readonly ApplicationContext db;
        public AnimalVisitedLocationController(ApplicationContext db)
        {
            this.db = db;
        }

        [HttpGet("{animalId:long?}/locations")]
        public ActionResult<IEnumerable<ResponseAnimalVisitedLocation>> GetAnimalVisitedLocationById(
            [Required][GreaterEqual(1)] long? animalId,
            [FromQuery] DateTimeOffset? startDateTime,
            [FromQuery] DateTimeOffset? endDateTime,
            [FromQuery][GreaterEqual(0)] int from = 0,
            [FromQuery][GreaterEqual(1)] int size = 10)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Animal? animal = db.Animals.Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId);
            if (animal == null)
                return NotFound();
            var visitedLocation = (from vl in animal.VisitedLocations
                                   where startDateTime == null || vl.DateTimeOfVisitLocationPoint >= startDateTime
                                   where endDateTime == null || vl.DateTimeOfVisitLocationPoint <= endDateTime
                                   where vl.LocationPointId != animal.ChippingLocationId
                                   orderby vl.DateTimeOfVisitLocationPoint
                                   select vl).Skip(from).Take(size);
            return Ok(Array.ConvertAll(visitedLocation.ToArray(), a => new ResponseAnimalVisitedLocation(a)));
        }

        [HttpPost("{animalId:long?}/locations/{pointId:long?}")]
        public ActionResult<ResponseAnimalVisitedLocation> AddAnimalVisitedLocationById([Required][GreaterEqual(1)] long? animalId, [Required][GreaterEqual(1)] long? pointId)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? animal = db.Animals.Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId);
            LocationPoint? p = db.Locations.FirstOrDefault(a => a.Id == pointId);
            if (animal == null || p == null)
                return NotFound();
            if (animal.LifeStatus == "DEAD" || (animal.VisitedLocations.Count == 0 && animal.ChippingLocationId == pointId) || (animal.VisitedLocations.Count >= 1 && animal.VisitedLocations.TakeLast(1).ToArray()[0].LocationPointId == pointId))
                return BadRequest();
            AnimalVisitedLocation avl = new () { DateTimeOfVisitLocationPoint = DateTimeOffsetTrunc.Truncate(DateTimeOffset.Now), LocationPointId = pointId.Value };
            animal.VisitedLocations.Add(avl);
            db.SaveChanges();
            return CreatedAtRoute("", new ResponseAnimalVisitedLocation(avl));
        }

        [HttpPut("{animalId:long?}/locations")]
        public ActionResult<ResponseAnimalVisitedLocation> UpdateAnimalVisitedLocationById([Required][GreaterEqual(1)] long? animalId, [FromBody] RequestAnimalVisitedLocation requestAnimalVisitedLocation)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? animal = db.Animals.Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId);
            AnimalVisitedLocation? avl1 = db.AnimalVisitedLocations.FirstOrDefault(a => a.Id == requestAnimalVisitedLocation.VisitedLocationPointId);
            LocationPoint? lp = db.Locations.FirstOrDefault(a => a.Id == requestAnimalVisitedLocation.LocationPointId);
            if (animal == null || avl1 == null || lp == null)
                return NotFound();
            AnimalVisitedLocation? avl = animal.VisitedLocations.FirstOrDefault(a => a.Id == requestAnimalVisitedLocation.VisitedLocationPointId);
            if (avl == null)
                return NotFound();
            int i1 = animal.VisitedLocations.IndexOf(avl) - 1;
            int i2 = i1 + 2;
            AnimalVisitedLocation? avli1 = animal.VisitedLocations.ElementAtOrDefault(i1);
            AnimalVisitedLocation? avli2 = animal.VisitedLocations.ElementAtOrDefault(i2);
            if (animal.ChippingLocationId == lp.Id || (animal.VisitedLocations.Count >= 1 && animal.VisitedLocations.LastOrDefault().LocationPointId == requestAnimalVisitedLocation.LocationPointId) ||
                (avli1 != null && avli1.LocationPointId == requestAnimalVisitedLocation.LocationPointId) ||
                (avli2 != null && avli2.LocationPointId == requestAnimalVisitedLocation.LocationPointId) ||
                avl.LocationPointId == requestAnimalVisitedLocation.LocationPointId)
                return BadRequest();
            avl.LocationPointId = lp.Id;
            db.SaveChanges();
            return Ok(new ResponseAnimalVisitedLocation(avl));
        }

        [HttpDelete("{animalId:long?}/locations/{visitedPointId:long?}")]
        public ActionResult DeleteAnimalVisitedLocationById([Required][GreaterEqual(1)] long? animalId, [Required][GreaterEqual(1)] long? visitedPointId) {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            Animal? animal = db.Animals.Include(a => a.VisitedLocations).FirstOrDefault(a => a.Id == animalId);
            AnimalVisitedLocation? avl = db.AnimalVisitedLocations.FirstOrDefault(a => a.Id == visitedPointId);
            if (animal == null || avl == null)
                return NotFound();
            if (!animal.VisitedLocations.Contains(avl))
                return NotFound();
            int i1 = animal.VisitedLocations.IndexOf(avl);
            if (i1 == 0 && animal.VisitedLocations.Count != 1) {
                AnimalVisitedLocation location = animal.VisitedLocations.ElementAt(1);
                if (location.LocationPointId == animal.ChippingLocationId) {
                    animal.VisitedLocations.Remove(location);
                }
            }
            animal.VisitedLocations.Remove(avl);
            db.SaveChanges();
            return Ok();
        }
    }
}
