using System.ComponentModel.DataAnnotations;
using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestLocationPoint
    {
        public RequestLocationPoint() { }
        public RequestLocationPoint(LocationPoint p)
        {
            Latitude = p.Latitude;
            Longitude = p.Longitude;
        }
        [Required]
        [Range(-90.0, 90.0)]
        public double? Latitude { get; set; }
        [Required]
        [Range(-180.0, 180.0)]
        public double? Longitude { get; set; }
    }
}
