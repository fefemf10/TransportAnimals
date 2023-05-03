using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseArea
    {
        public ResponseArea() { }
        public ResponseArea(Area area)
        {
            Id = area.Id;
            Name = area.Name;
            AreaPoints = area.AreaPoints.ConvertAll(a => new RequestLocationPoint(a));
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<RequestLocationPoint> AreaPoints { get; set; }
    }
}
