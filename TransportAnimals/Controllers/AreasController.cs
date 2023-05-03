using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;

namespace TransportAnimals.Controllers
{
    [Route("areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ApplicationContext db;
        public AreasController(ApplicationContext db)
        {
            this.db = db;
        }

        [HttpGet("{areaId:long?}")]
        public ActionResult GetAreaById([Required][GreaterEqual(1)] long? areaId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Area? area = db.Areas.Include(a => a.AreaPoints).FirstOrDefault(a => a.Id == areaId);
            return area != null ? Ok(new ResponseArea(area)) : NotFound();
        }

        [HttpPost]
        public ActionResult AddArea([FromBody] RequestArea area)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            if (!requester.IsAdmin)
                return Forbid();
            //такое же имя
            if (db.Areas.Any(a => a.Name == area.Name))
            {
                return Conflict();
            }
            //имеют одинаковые точки
            bool hasSamePoints = db.Areas.Include(d => d.AreaPoints).ToList().Any(a => a.AreaPoints.Any(b => area.AreaPoints.Any(c => c.Latitude == b.Latitude && c.Longitude == b.Longitude)));
            //все точки лежат на 1 прямой или какая-то из точек лежит внутри другой зоны
            if (Geometry.InLineAllPoints(area.AreaPoints))
                return BadRequest("All points in line");
            //лежит ли какая то точка в другой зоне
            bool hasPointInAnotherZone = db.Areas.Include(d => d.AreaPoints).ToList().Any(a =>
            {
                var arr = a.AreaPoints.ConvertAll(c => new RequestLocationPoint(c)).ToArray();
                if (Geometry.TestAABB(arr, area.AreaPoints))
                {
                    return area.AreaPoints.Any(b => Geometry.InPolygon(hasSamePoints, arr, b));
                }
                return false;
            });
            //лежат ли какие либо точки другой зоны в этой зоне
            bool hasPointsAnotherZoneinCurrentZone = db.Areas.Include(d => d.AreaPoints).ToList().Any(a =>
            {
                if (Geometry.TestAABB(area.AreaPoints, a.AreaPoints.ConvertAll(c => new RequestLocationPoint(c)).ToArray()))
                {
                    return a.AreaPoints.Any(b => Geometry.InPolygon(hasSamePoints, area.AreaPoints, new RequestLocationPoint(b)));
                }
                return false;
            });
            if (hasPointInAnotherZone || hasPointsAnotherZoneinCurrentZone)
            {
                return BadRequest();
            }
            //само пересекающийся полигон
            if (Geometry.IsSelfIntersecting(area.AreaPoints))
            {
                return BadRequest("IntersectPolygon");
            }
            Area a = new() { Name = area.Name, AreaPoints = Array.ConvertAll(area.AreaPoints, a => new LocationPoint(a)).ToList() };
            db.Areas.Add(a);
            db.SaveChanges();
            return Created("", new ResponseArea(a));
        }

        [HttpPut("{areaId:long?}")]
        public ActionResult UpdateArea([Required][GreaterEqual(1)] long? areaId, [FromBody] RequestArea area)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            if (!requester.IsAdmin)
                return Forbid();
            //такое же имя
            if (db.Areas.Any(a => a.Name == area.Name))
            {
                return Conflict();
            }
            //имеют одинаковые точки
            bool hasSamePoints = db.Areas.Include(d => d.AreaPoints).ToList().Any(a => a.AreaPoints.Any(b => area.AreaPoints.Any(c => c.Latitude == b.Latitude && c.Longitude == b.Longitude)));
            //все точки лежат на 1 прямой или какая-то из точек лежит внутри другой зоны
            if (Geometry.InLineAllPoints(area.AreaPoints))
                return BadRequest("All points in line");
            //лежит ли какая то точка в другой зоне
            bool hasPointInAnotherZone = db.Areas.Include(d => d.AreaPoints).ToList().Any(a =>
            {
                var arr = a.AreaPoints.ConvertAll(c => new RequestLocationPoint(c)).ToArray();
                if (Geometry.TestAABB(arr, area.AreaPoints))
                {
                    return area.AreaPoints.Any(b => Geometry.InPolygon(hasSamePoints, arr, b));
                }
                return false;
            });
            //лежат ли какие либо точки другой зоны в этой зоне
            bool hasPointsAnotherZoneinCurrentZone = db.Areas.Include(d => d.AreaPoints).ToList().Any(a =>
            {
                if (Geometry.TestAABB(area.AreaPoints, a.AreaPoints.ConvertAll(c => new RequestLocationPoint(c)).ToArray()))
                {
                    return a.AreaPoints.Any(b => Geometry.InPolygon(hasSamePoints, area.AreaPoints, new RequestLocationPoint(b)));
                }
                return false;
            });
            if (hasPointInAnotherZone || hasPointsAnotherZoneinCurrentZone)
            {
                return BadRequest();
            }
            //само пересекающийся полигон
            if (Geometry.IsSelfIntersecting(area.AreaPoints))
            {
                return BadRequest("IntersectPolygon");
            }
            Area a = new() { Name = area.Name, AreaPoints = Array.ConvertAll(area.AreaPoints, a => new LocationPoint(a)).ToList() };
            db.Areas.Add(a);
            db.SaveChanges();
            return Ok(new ResponseArea(a));
        }

        [HttpDelete("{areaId:long?}")]
        public ActionResult DeleteAreaById([Required][GreaterEqual(1)] long? areaId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            if (!requester.IsAdmin)
                return Forbid();
            Area? area = db.Areas.FirstOrDefault(a => a.Id == areaId);
            if (area != null)
            {
                db.Areas.Remove(area);
                db.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }
        [HttpGet("{areaId:long?}/analytics")]
        public ActionResult GetArealytics([FromQuery][Required] DateOnly? startDate, [FromQuery][Required] DateOnly? endDate, [Required][GreaterEqual(1)] long? areaId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Area? area = db.Areas.Include(a => a.AreaPoints).FirstOrDefault(a => a.Id == areaId);
            if(area == null)
                return NotFound();
            if (startDate >= endDate)
                return BadRequest();

            //animals[i].visloc[j].date between start and end date && locationPointId == areaId
            int totalQuantityAnimals = db.Animals.Include(a => a.VisitedLocations).ToList().Count(i => i.VisitedLocations.Any(j =>
            {
                bool date = DateOnly.FromDateTime(j.DateTimeOfVisitLocationPoint.UtcDateTime) >= startDate && DateOnly.FromDateTime(j.DateTimeOfVisitLocationPoint.UtcDateTime) <= endDate;
                RequestLocationPoint point = new RequestLocationPoint(db.Locations.FirstOrDefault(l => l.Id == j.LocationPointId));
                RequestLocationPoint[] areasPoints = area.AreaPoints.ConvertAll(a => new RequestLocationPoint(a)).ToArray();
                if (date && Geometry.TestAABB(areasPoints, point))
                {
                    return Geometry.InPolygon(true, areasPoints, point);
                }
                return false;
            }));
            return Ok();
        }
    }
}
