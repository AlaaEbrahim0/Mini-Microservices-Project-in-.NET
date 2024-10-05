using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SynDataServices.Grpc;

public interface IPlatformDataClient
{
	IEnumerable<Platform> ReturnAllPlatforms();
}

public class PlatformDataClient : IPlatformDataClient
{
	private readonly IConfiguration _configruation;
	private readonly IMapper _mapper;
	public PlatformDataClient(IConfiguration configuration, IMapper mapper)
	{
		_configruation = configuration;
		_mapper = mapper;

	}
	public IEnumerable<Platform> ReturnAllPlatforms()
	{
		Console.WriteLine($"--> Calling GRPC Service {_configruation["GrpcPlatform"]}");
		var channel = GrpcChannel.ForAddress(_configruation["GrpcPlatform"]);
		var client = new GrpcPlatform.GrpcPlatformClient(channel);
		var request = new GetAllRequest();

		try
		{
			var result = client.GetAllPlatforms(request);
			var platforms = _mapper.Map<IEnumerable<Platform>>(result.Platform);
			return platforms;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"--> Could not call GRPC Server {ex.Message}");
			return new List<Platform>();
		}

	}
}
