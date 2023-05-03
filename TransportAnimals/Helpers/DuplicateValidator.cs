using System.ComponentModel.DataAnnotations;
using TransportAnimals.Models;

namespace TransportAnimals.Helpers
{
    public static class DuplicateValidator<T>
    {
        public static ValidationResult HasDuplicate(IEnumerable<T> value, ValidationContext context)
        {
            if (!(value.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToArray().Length > 0))
                return ValidationResult.Success;
            return new ValidationResult("Has duplicates");
        }
    }
}
