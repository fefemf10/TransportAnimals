using System.ComponentModel.DataAnnotations;
using TransportAnimals.Helpers;

namespace TransportAnimals.ViewModels.Request
{
    public class RequestUpdateAccount
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EnumDataType(typeof(Role))]
        public string? Role { get; set; }
    }
}
