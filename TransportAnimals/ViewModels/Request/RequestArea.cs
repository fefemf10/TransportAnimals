using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;
using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestArea
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [MinLength(3)]
        [CustomValidation(typeof(DuplicateValidator<RequestLocationPoint>), "HasDuplicate")]
        public RequestLocationPoint[]? AreaPoints { get; set; }
    }
}
