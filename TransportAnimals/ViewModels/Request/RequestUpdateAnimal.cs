using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestUpdateAnimal
    {
        [Required]
        [Greater(0)]
        public double? Weight { get; set; }
        [Required]
        [Greater(0)]
        public double? Length { get; set; }
        [Required]
        [Greater(0)]
        public double? Height { get; set; }
        [Required]
        [EnumDataType(typeof(Gender))]
        public string? Gender { get; set; }
        [Required]
        [EnumDataType(typeof(LifeStatus))]
        public string? LifeStatus { get; set; }
        [Required]
        [GreaterEqual(1)]
        public int? ChipperId { get; set; }
        [Required]
        [GreaterEqual(1)]
        public long? ChippingLocationId { get; set; }
    }
}
