using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Services;
using TicTacToe.Web.Data;
using TicTacToe.Web.Endpoints;
using TicTacToe.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
{
	options.UseSqlite(builder.Configuration.GetConnectionString("TicTacToe"));
});
builder.Services.AddScoped<IGameEngine, GameEngine>();
builder.Services.AddScoped<IMatchSessionRepository, MatchSessionRepository>();
builder.Services.AddScoped<IBrowserIdentityService, BrowserIdentityCookieService>();
builder.Services.AddScoped<IMatchRefereeService, MatchRefereeService>();
builder.Services.AddSingleton<RepositoryFaultInjectionState>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
		var problemDetails = context.RequestServices.GetRequiredService<IProblemDetailsService>();
		var statusCode = exception is MatchPersistenceException ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status500InternalServerError;

		context.Response.StatusCode = statusCode;

		await problemDetails.WriteAsync(new ProblemDetailsContext
		{
			HttpContext = context,
			ProblemDetails = new ProblemDetails
			{
				Status = statusCode,
				Title = statusCode == StatusCodes.Status503ServiceUnavailable ? "Match state unavailable" : "Unhandled server error",
				Detail = exception?.Message ?? "An unexpected error occurred.",
			},
		});
	});
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
	await dbContext.Database.MigrateAsync();
}

app.MapMatchEndpoints();
app.MapFallbackToFile("index.html");

await app.RunAsync();

public partial class Program;
