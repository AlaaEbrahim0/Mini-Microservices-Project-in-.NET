using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data;

public static class PrepDb
{
	public static void PrepPopulation(this IApplicationBuilder app, bool isProductionEnv)
	{
		using (var serviceScope = app.ApplicationServices.CreateScope())
		{
			var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
			SeedData(dbContext!, isProductionEnv);
		}

	}

	private static void SeedData(AppDbContext dbContext, bool isProductionEnv)
	{
		if (isProductionEnv){
			try{
				Console.WriteLine("---> Attempting to apply migrations");
				dbContext.Database.Migrate();
			}
			catch(Exception ex){
				Console.WriteLine($"---> Could not run migrations: {ex.Message}");
			}
		}
		if (!dbContext.Platforms.Any())
		{
			Console.WriteLine("--> Seeding Data <--");
			dbContext.Platforms.AddRange
			(
				new Platform { Name = "Dotnet", Publisher = "Microsoft", Cost = "Free" },
				new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
				new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
			);

			dbContext.SaveChanges();
		}
		else
		{
			Console.WriteLine("--> We already have data <--");
		}
	}
}
