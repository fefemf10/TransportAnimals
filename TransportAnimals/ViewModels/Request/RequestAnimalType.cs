using System.ComponentModel.DataAnnotations;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestAnimalType
    {
        [Required]
        public string? Type { get; set; }
    }
}
