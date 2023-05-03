using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportAnimals.Helpers;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;
using TransportAnimals.ViewModels.Response;

namespace TransportAnimals.Controllers
{
    [Route("registration")]
	[ApiController]
	public class RegistrationController : ControllerBase
	{
		private readonly ApplicationContext db;
		public RegistrationController(ApplicationContext db)
		{
			this.db = db;
		}

		[HttpPost]
		public ActionResult<ResponseAccount> Post([FromBody] RequestAccount account)
		{
            bool? requester = AnonymousAuth.isValidAnon(db, Request.Headers.Authorization);
            if (requester != null)
			{
				return requester.Value ? Forbid() : BadRequest();
            }
            Account? acc = db.Accounts.FirstOrDefault(acc => acc.Email == account.Email);
			if (acc != null)
				return Conflict();
			else
			{
				acc = new Account() { FirstName = account.FirstName, LastName = account.LastName, Email = account.Email, Password = account.Password, Role = "USER" };
				db.Accounts.Add(acc);
				db.SaveChanges();
				return CreatedAtAction(null, new ResponseAccount(acc));
			}
		}
	}
}
