﻿using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepository
{
	bool SaveChanges();

	IEnumerable<Platform> GetAllPlatforms();
	void CreatePlatform(Platform plat);
	bool PlatformExists(int platformId);

	IEnumerable<Command> GetCommandsForPlatform(int platformId);
	Command? GetCommand(int platformId, int commandId);
	void CreateCommand(int platformId, Command command);

	bool ExternalPlatformExists(int externalPlatformId);

}
