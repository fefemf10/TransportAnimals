namespace TransportAnimals.ViewModels.Response
{
    public class ResponseAnimalAnalytics
    {
        public string AnimalType { get; set; }
        public long AnimalTypeID { get; set; }
        public long QuantityAnimals { get; set; }
        public long AnimalsArrived { get; set; }
        public long AnimalsGone { get; set; }
    }
    public class ResponseAnalytics
    {
        public long TotalQuantityAnimals { get; set; }
        public long TotalAnimalsArrived { get; set; }
        public long TotalAnimalsGone { get; set; }
        public ResponseAnimalAnalytics[] AnimalAnalytics { get; set; }
    }
}
