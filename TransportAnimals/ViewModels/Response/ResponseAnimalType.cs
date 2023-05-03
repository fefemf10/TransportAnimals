using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseAnimalType
    {
        public ResponseAnimalType() { }
        public ResponseAnimalType(AnimalType animalType)
        {
            Id = animalType.Id;
            Type = animalType.Type;
        }
        public long Id { get; set; }
        public string Type { get; set; }
    }
}
