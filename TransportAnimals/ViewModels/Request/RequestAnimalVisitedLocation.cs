using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestAnimalVisitedLocation
    {
        [Required]
        [GreaterEqual(1)]
        public long? VisitedLocationPointId { get; set; }
        [Required]
        [GreaterEqual(1)]
        public long? LocationPointId { get; set; }
    }
}
