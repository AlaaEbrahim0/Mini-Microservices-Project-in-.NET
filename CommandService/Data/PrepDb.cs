using CommandService.Models;
using CommandService.SynDataServices.Grpc;

namespace CommandService.Data;

public static class PrepDb
{
	public static void PrepPopulation(this IApplicationBuilder app)
	{
		using (var serviceScope = app.ApplicationServices.CreateScope())
		{
			var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
			var platforms = grpcClient.ReturnAllPlatforms();

			var repo = serviceScope.ServiceProvider.GetService<ICommandRepository>();
			SeedData(repo, platforms);
		}

	}

	private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
	{
		Console.WriteLine("Seeding new platforms");
		foreach (var platform in platforms)
		{
			if (!repository.ExternalPlatformExists(platform.ExternalId))
			{
				repository.CreatePlatform(platform);
			}
			repository.SaveChanges();
		}
	}
}
