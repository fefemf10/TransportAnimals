using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseAnimal
    {
        public ResponseAnimal() { }
        public ResponseAnimal(Animal animal)
        {
            Id = animal.Id;
            AnimalTypes = animal.AnimalTypes.Where(a => a != null).Select(a => a.Id).ToArray();
            Weight = animal.Weight;
            Length = animal.Length;
            Height = animal.Height;
            Gender = animal.Gender;
            LifeStatus = animal.LifeStatus;
            ChippingDateTime = animal.ChippingDateTime;
            ChipperId = animal.ChipperId;
            ChippingLocationId = animal.ChippingLocationId;
            VisitedLocations = animal.VisitedLocations.Where(a => a != null).Select(a => a.Id).ToArray();
            DeathDateTime = animal.DeathDateTime;
        }
        public long Id { get; set; }
        public long[] AnimalTypes { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public string Gender { get; set; }
        public string LifeStatus { get; set; }
        public DateTimeOffset? ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; }
        public DateTimeOffset? DeathDateTime { get; set; }
    }
}
