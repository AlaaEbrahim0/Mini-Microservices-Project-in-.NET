using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformController : ControllerBase
{
	private readonly IPlatformRepository _repo;
	private readonly IMapper _mapper;
	private readonly ICommandDataClient _commandDataClient;

	public PlatformController(IPlatformRepository repo, IMapper mapper, ICommandDataClient commandDataClient)
	{
		_repo = repo;
		_mapper = mapper;
		_commandDataClient = commandDataClient;
	}

	[HttpGet]
	public IActionResult GetPlatforms()
	{
		var platforms = _repo.GetAllPlatforms();
		var platformsDto = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
		return Ok(platformsDto);
	}

	[HttpGet("{id:int}")]
	public IActionResult GetPlatformById(int id)
	{
		var platform = _repo.GetPlatformById(id);
		if (platform is null)
		{
			return NotFound();
		}

		var platformDto = _mapper.Map<PlatformReadDto>(platform);
		return Ok(platformDto);
	}

	[HttpPost]
	public async Task<IActionResult> CreatePlatform(PlatformCreateDto platformToCreate)
	{
		var platform = _mapper.Map<Platform>(platformToCreate);
		_repo.CreatePlatform(platform);
		_repo.SaveChanges();

		var platformDto = _mapper.Map<PlatformReadDto>(platform);

		try
		{
			await _commandDataClient.SendPlatformToCommand(platformDto);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Could not send synchronously: {ex.Message}");
		}

		return CreatedAtAction(nameof(GetPlatformById), new { id = platform.Id }, platform);

	}

	[HttpGet]
	[Route("/dummy")]
	public IActionResult DummyEndpoint([FromQuery] bool isFailure)
	{
		if (isFailure)
		{
			return BadRequest();
		}
		return Ok();
	}

}
