using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseAnimalVisitedLocation
    {
        public ResponseAnimalVisitedLocation()
        {

        }
        public ResponseAnimalVisitedLocation(AnimalVisitedLocation animalVisitedLocation)
        {
            Id = animalVisitedLocation.Id;
            DateTimeOfVisitLocationPoint = animalVisitedLocation.DateTimeOfVisitLocationPoint;
            LocationPointId = animalVisitedLocation.LocationPointId;
        }
        public long Id { get; set; }
        public DateTimeOffset DateTimeOfVisitLocationPoint { get; set; }
        public long LocationPointId { get; set; }
    }
}
