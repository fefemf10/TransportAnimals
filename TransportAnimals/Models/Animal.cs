
namespace TransportAnimals.Models
{
    public class Animal
    {
        public long Id { get; set; }
        public List<AnimalType> AnimalTypes { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public string Gender { get; set; }
        public string LifeStatus { get; set; }
        public DateTimeOffset? ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public Account Chipper { get; set; }
        public long ChippingLocationId { get; set; }
        public LocationPoint ChippingLocation { get; set; }
        public List<AnimalVisitedLocation> VisitedLocations { get; set; }
        public List<AnimalAnimalVisitedLocation> AnimalVisitedLocations { get; set; }
        public DateTimeOffset? DeathDateTime { get; set; }
    }
}