using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;
[ApiController]
[Route("api/c/platforms/{platformId}/commands")]
public class CommandController : ControllerBase
{
	private readonly ICommandRepository _repository;
	private readonly IMapper _mapper;

	public CommandController(ICommandRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpGet]
	public IActionResult GetCommandsForPlatform([FromRoute] int platformId)
	{
		Console.WriteLine("--> Getting Commands from Command Service....");
		var platform = _repository.PlatformExists(platformId);

		if (!platform) return NotFound();

		var commands = _repository.GetCommandsForPlatform(platformId);
		var result = _mapper.Map<IEnumerable<CommandReadDto>>(commands);
		return Ok(result);
	}

	[HttpGet("{commandId:int}")]
	public IActionResult GetCommand(int platformId, int commandId)
	{
		Console.WriteLine("--> Getting Commands from Command Service....");
		var platform = _repository.PlatformExists(platformId);

		if (!platform) return NotFound();

		var command = _repository.GetCommand(platformId, commandId);

		if (command is null) return NotFound();

		var result = _mapper.Map<CommandReadDto>(command);
		return Ok(result);
	}

	[HttpPost]
	public IActionResult CreateCommand(int platformId, CommandCreateDto commandDto)
	{
		Console.WriteLine("--> Creating Commands from Command Service....");

		if (!_repository.PlatformExists(platformId)) return NotFound();

		var command = _mapper.Map<Command>(commandDto);
		_repository.CreateCommand(platformId, command);
		_repository.SaveChanges();

		var result = _mapper.Map<CommandReadDto>(command);
		return CreatedAtAction(
			nameof(GetCommand),
			new { platformId = platformId, commandId = result.Id },
			result);

	}


}

