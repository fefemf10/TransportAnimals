using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseLocationPoint
    {
        public ResponseLocationPoint() { }
        public ResponseLocationPoint(LocationPoint point)
        {
            Id = point.Id;
            Latitude = point.Latitude;
            Longitude = point.Longitude;
        }
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
