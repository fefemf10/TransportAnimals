using TransportAnimals.Helpers;
using TransportAnimals.Models;

namespace TransportAnimals.ViewModels.Response
{
    public class ResponseAccount
    {
        public ResponseAccount() { }
        public ResponseAccount(Account account)
        {
            Id = account.Id;
            FirstName = account.FirstName;
            LastName = account.LastName;
            Email = account.Email;
            Role = account.Role;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
