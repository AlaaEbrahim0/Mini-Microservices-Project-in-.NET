using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
	private readonly HttpClient httpClient;
	private readonly IConfiguration configuration;

	public HttpCommandDataClient(IConfiguration configuration, HttpClient httpClient)
	{

		this.configuration = configuration;
		this.httpClient = httpClient;
	}

	public async Task SendPlatformToCommand(PlatformReadDto dto)
	{
		var httpContent = new StringContent(
			JsonSerializer.Serialize(dto),
			Encoding.UTF8,
			"application/json"
		);

		var url = configuration.GetValue<string>("CommandServiceUrl");

		var response = await httpClient.PostAsync(url, httpContent);

		if (response.IsSuccessStatusCode)
		{
			Console.WriteLine("--> Sync POST to Command Service succeeded");
		}
		else
		{
        var responseBody = await response.Content.ReadAsStringAsync();
				throw new Exception(responseBody);
		}

	}
}
