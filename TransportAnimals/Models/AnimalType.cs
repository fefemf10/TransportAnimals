namespace TransportAnimals.Models
{
    public class AnimalType
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public List<Animal> Animals { get; set; }
    }
}
