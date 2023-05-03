using TransportAnimals.ViewModels.Request;

namespace TransportAnimals.Models
{
    public class LocationPoint
    {
        public LocationPoint() { }
        public LocationPoint(RequestLocationPoint p)
        {
            Latitude = p.Latitude.Value;
            Longitude = p.Longitude.Value;
        }
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<AnimalVisitedLocation> AnimalVisitedLocations { get; set; }
        public List<Animal> Animals { get; set; }
        public List<Area> Areas { get; set; } = new();
    }
}
