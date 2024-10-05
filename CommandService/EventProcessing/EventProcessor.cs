using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly IMapper _mapper;

	public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
	{
		_serviceScopeFactory = serviceScopeFactory;
		_mapper = mapper;
	}

	public void ProcessEvent(string message)
	{
		var eventType = DetermineEventType(message);
		switch (eventType)
		{
			case EventType.PlatformPublished:
				AddPlatform(message);
				break;

			default:
				break;
		}
	}

	private void AddPlatform(string message)
	{
		using (var scope = _serviceScopeFactory.CreateScope())
		{
			var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
			var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);

			try
			{
				var platform = _mapper.Map<Platform>(platformPublishedDto);
				if (!commandRepo.ExternalPlatformExists(platform.ExternalId))
				{
					commandRepo.CreatePlatform(platform);
					commandRepo.SaveChanges();
				}

				else
				{
					Console.WriteLine("Platform already exists!");
				}

			}
			catch (Exception ex)
			{

				throw;
			}
		}
	}

	private EventType DetermineEventType(string notificationMessage)
	{
		Console.WriteLine("Determining Event");

		var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
		switch (eventType.Event)
		{
			case "Platform_Published":
				return EventType.PlatformPublished;

			default:
				Console.WriteLine("Could not determine the event type");
				return EventType.Undetermined;
		}
	}
}
