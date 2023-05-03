namespace TransportAnimals.Models
{
    public class AnimalVisitedLocation
    {
        public long Id { get; set; }
        public DateTimeOffset DateTimeOfVisitLocationPoint { get; set; }
        public long LocationPointId { get; set; }
        public List<Animal> Animals { get; set; }
        public List<AnimalAnimalVisitedLocation> AnimalVisitedLocations { get; set; }
    }
}
