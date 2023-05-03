using idunno.Authentication.Basic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using TransportAnimals.Helpers;
using TransportAnimals.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
//	.AddBasic(options =>
//	{
//		options.Realm = "Basic Authentication";
//		options.Events = new BasicAuthenticationEvents
//		{
//			OnValidateCredentials = context =>
//			{
//				ApplicationContext? db = context.HttpContext.RequestServices.GetService<ApplicationContext>();
//				Account? account = db.Accounts.FirstOrDefault(a => a.Email == context.Username && a.Password == context.Password);
//				if (account != null)
//				{
//					var claims = new[]
//					{
//						new Claim(ClaimTypes.Name, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer),
//						new Claim(ClaimTypes.Email, context.Username, ClaimValueTypes.Email, context.Options.ClaimsIssuer),
//					};
//					context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
//					context.Success();
//                }
//				return Task.CompletedTask;
//			}
//		};
//	});
builder.Services.AddAuthentication(options => {
	options.DefaultChallengeScheme = "scheme name";

	// you can also skip this to make the challenge scheme handle the forbid as well
	options.DefaultForbidScheme = "scheme name";

	// of course you also need to register that scheme, e.g. using
	options.AddScheme<MySchemeAuth>("scheme name", "scheme display name");
}
);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
}
  );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDateOnlyTimeOnlyStringConverters();
var host = builder.Configuration["DBHOST"] ?? "localhost";
var port = builder.Configuration["DBPORT"] ?? "5432";
var user = builder.Configuration["DBUSER"] ?? "postgres";
var password = builder.Configuration["DBPASSWORD"] ?? "1234";
builder.Services.AddDbContext<ApplicationContext>(
					options =>
					{
						string connection = $"Host={host};Port={port};Database=transportanimals;Username={user};Password={password};Include Error Detail=true";
						options.UseNpgsql(connection);
					});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    DbInitializer.Initialize(dbContext);
}
app.UseAuthorization();
app.UseCors(builder => builder.AllowAnyOrigin());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.MapControllers();
app.Run();