using TransportAnimals.ViewModels.Request;

namespace TransportAnimals.Models
{
    public class Area
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<LocationPoint> AreaPoints { get; set; } = new();
    }
}
