using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text.RegularExpressions;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;

namespace TransportAnimals.Controllers
{
    [Route("locations")]
    [ApiController]
    public class LocationPointController : ControllerBase
    {
        private readonly ApplicationContext db;
        public LocationPointController(ApplicationContext db)
        {
            this.db = db;
        }
        [HttpGet("{pointId:long?}")]
        public ActionResult<ResponseLocationPoint> GetLocationPointById([Required][GreaterEqual(1)] long? pointId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            LocationPoint? p = db.Locations.FirstOrDefault(p => p.Id == pointId);
            return p != null ? Ok(new ResponseLocationPoint(p)) : NotFound();
        }

        [HttpPost]
        public ActionResult<ResponseLocationPoint> AddLocationPoint([FromBody] RequestLocationPoint p)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            LocationPoint? point = db.Locations.FirstOrDefault(point => point.Latitude == p.Latitude && point.Longitude == p.Longitude);
            if (point != null)
                return Conflict();
            else
            {
                point = new LocationPoint() { Latitude = p.Latitude.Value, Longitude = p.Longitude.Value };
                db.Locations.Add(point);
                db.SaveChanges();
                return CreatedAtRoute("", new ResponseLocationPoint(point));
            }
        }

        [HttpPut("{pointId:long?}")]
        public ActionResult<ResponseLocationPoint> UpdateLocationPointById([Required][GreaterEqual(1)] long? pointId, [FromBody] RequestLocationPoint p)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            LocationPoint? point = db.Locations.FirstOrDefault(point => point.Latitude == p.Latitude && point.Longitude == p.Longitude);
            if (point != null)
                return Conflict();
            point = db.Locations.FirstOrDefault(p => p.Id == pointId);
            if (point == null)
                return NotFound();
            point.Latitude = p.Latitude.Value;
            point.Longitude = p.Longitude.Value;
            db.SaveChanges();
            return Ok(new ResponseLocationPoint(point));
        }

        [HttpDelete("{pointId:long?}")]
        public ActionResult DeleteLocationPointById([Required][GreaterEqual(1)] long? pointId)
        {
            //if (!AnonymousAuth.isValid(db, Request.Headers.Authorization))
            //    return Unauthorized();
            LocationPoint? point = db.Locations.FirstOrDefault(p => p.Id == pointId.Value);
            if (point == null)
                return NotFound();
            if (db.Animals.Any(a => a.VisitedLocations.Any(t => t.LocationPointId == pointId.Value)) || db.Animals.Any(a => a.ChippingLocationId == pointId.Value))
                return BadRequest();
            db.Locations.Remove(point);
            db.SaveChanges();
            return Ok();
        }
    }
}
