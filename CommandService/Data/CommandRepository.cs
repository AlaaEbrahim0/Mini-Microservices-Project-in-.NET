using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
	private readonly AppDbContext _context;
	public CommandRepository(AppDbContext context)
	{
		_context = context;
	}
	public void CreateCommand(int platformId, Command command)
	{
		if (command is null)
		{
			throw new ArgumentNullException(nameof(command));
		}

		command.PlatformId = platformId;
		_context.Commands.Add(command);
	}

	public void CreatePlatform(Platform plat)
	{
		if (plat is null)
		{
			throw new ArgumentNullException(nameof(plat));
		}
		_context.Platforms.Add(plat);

	}

	public bool ExternalPlatformExists(int externalPlatformId)
	{
		return _context.Platforms.Any(x => x.ExternalId == externalPlatformId);

	}

	public IEnumerable<Platform> GetAllPlatforms()
	{
		return _context.Platforms.ToList();
	}

	public Command? GetCommand(int platformId, int commandId)
	{
		return _context.Commands
			.Where(x => x.PlatformId == platformId && x.Id == commandId)
			.FirstOrDefault();
	}

	public IEnumerable<Command> GetCommandsForPlatform(int platformId)
	{
		var commands = _context.Commands
			.Where(x => x.PlatformId == platformId)
			.OrderBy(x => x.Platform.Name);

		return commands;
	}

	public bool PlatformExists(int platformId)
	{
		return _context.Platforms.Any(x => x.Id == platformId);
	}

	public bool SaveChanges()
	{
		return _context.SaveChanges() > 0 ? true : false;
	}
}
