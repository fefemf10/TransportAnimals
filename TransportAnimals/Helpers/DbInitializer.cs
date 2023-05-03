using Microsoft.EntityFrameworkCore;
using TransportAnimals.Models;

namespace TransportAnimals.Helpers
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            if (context.Accounts.Any())
            {
                return;
            }
            context.Accounts.AddRange(new Account[]
                {
                    new Account { FirstName = "adminFirstName", LastName = "adminLastName", Email = "admin@simbirsoft.com", Password = "qwerty123", Role = "ADMIN", Animals = new List<Animal>() },
                    new Account { FirstName = "chipperFirstName", LastName = "chipperLastName", Email = "chipper@simbirsoft.com", Password = "qwerty123", Role = "CHIPPER", Animals = new List<Animal>() },
                    new Account { FirstName = "userFirstName", LastName = "userLastName", Email = "user@simbirsoft.com", Password = "qwerty123", Role = "USER", Animals = new List<Animal>() }
                });
            context.SaveChanges();
        }
    }
}
