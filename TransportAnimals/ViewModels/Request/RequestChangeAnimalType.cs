using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestChangeAnimalType
    {
        [Required]
        [GreaterEqual(1)]
        public long? OldTypeId { get; set; }
        [Required]
        [GreaterEqual(1)]
        public long? NewTypeId { get; set; }
    }
}
