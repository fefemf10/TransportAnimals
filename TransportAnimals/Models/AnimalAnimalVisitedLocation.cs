namespace TransportAnimals.Models
{
    public class AnimalAnimalVisitedLocation
    {
        public long Id { get; set; }
        public long AnimalId { get; set; }
        public Animal Animal { get; set; }
        public long VisitedLocationId { get; set; }
        public AnimalVisitedLocation VisitedLocation { get; set; }

    }
}
