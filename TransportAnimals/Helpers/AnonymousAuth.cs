using TransportAnimals.Models;

namespace TransportAnimals.Helpers
{
    public class AnonymousAuth
    {
        public static bool? isValidAnon(ApplicationContext db, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            string[] data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Remove(0, 6))).Split(":");
            return db.Accounts.Any(a => a.Email == data[0] && a.Password == data[1]);
        }
        public static Account isValid(ApplicationContext db, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            string[] data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Remove(0, 6))).Split(":");
            return db.Accounts.FirstOrDefault(a => a.Email == data[0] && a.Password == data[1]);
        }
        public static string[] GetData(string value)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Remove(0, 6))).Split(":");
        }
    }
}
