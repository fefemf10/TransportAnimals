using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace TransportAnimals.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext db;
        public AccountController(ApplicationContext db)
        {
            this.db = db;
        }

        [HttpGet("{accountId:int?}")]
        public ActionResult<ResponseAccount> GetAccountById([Required][GreaterEqual(1)] int? accountId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Account? user = db.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (!requester.IsAdmin)
            {
                if (requester.Id == accountId)
                    return user != null ? Ok(new ResponseAccount(user)) : NotFound();
                else
                    return Forbid();
            }
            else
            {
                return user != null ? Ok(new ResponseAccount(user)) : NotFound();
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<ResponseAccount>> SearchAccount(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery][MinLength(1)] string? email,
            [FromQuery][GreaterEqual(0)] int from = 0,
            [FromQuery][GreaterEqual(1)] int size = 10)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            if (!requester.IsAdmin)
                return Forbid();
            var selectedAccounts = (from acc in db.Accounts
                                    where firstName == null || acc.FirstName.ToLower().Contains(firstName.ToLower())
                                    where lastName == null || acc.LastName.ToLower().Contains(lastName.ToLower())
                                    where email == null || acc.Email.ToLower().Contains(email.ToLower())
                                    orderby acc.Id
                                    select acc).Skip(from).Take(size);
            return Ok(Array.ConvertAll(selectedAccounts.ToArray(), a => new ResponseAccount(a)));
        }

        [HttpPost]
        public ActionResult<ResponseAccount> AddAccount([FromBody] RequestUpdateAccount account)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            if (!requester.IsAdmin)
                return Forbid();
            if (db.Accounts.Any(a => a.Email == account.Email))
                return Conflict();
            Account acc = new() { Email = account.Email, FirstName = account.FirstName, LastName = account.LastName, Password = account.Password, Role = account.Role, Animals = new List<Animal>() };
            db.Accounts.Add(acc);
            db.SaveChanges();
            return Created("", new ResponseAccount(acc));
        }

        [HttpPut("{accountId:int?}")]
        public ActionResult<ResponseAccount> UpdateAccountById([Required][GreaterEqual(1)] int? accountId, [FromBody] RequestUpdateAccount account)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Account? user = db.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (!requester.IsAdmin)
            {
                if (user == null || requester.Id != user.Id)
                    return Forbid();
            }
            else
            {
                if (user == null)
                    return NotFound();
            }
            if (user.Email != account.Email && db.Accounts.Any(a => a.Email == account.Email))
                return Conflict();
            user.Email = account.Email;
            user.FirstName = account.FirstName;
            user.LastName = account.LastName;
            user.Email = account.Email;
            user.Password = account.Password;
            user.Role = account.Role;
            db.SaveChanges();
            return Ok(new ResponseAccount(user));
        }

        [HttpDelete("{accountId:int?}")]
        public ActionResult DeleteAccountById([Required][GreaterEqual(1)] int? accountId)
        {
            Account requester = AnonymousAuth.isValid(db, Request.Headers.Authorization);
            if (requester == null)
                return Unauthorized();
            Account? user = db.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (!requester.IsAdmin)
            {
                if (user == null || requester.Id != user.Id)
                    return Forbid();
            }
            else
            {
                if (user == null)
                    return NotFound();
            }
            db.Accounts.Remove(user);
            db.SaveChanges();
            return Ok();
        }
    }
}
