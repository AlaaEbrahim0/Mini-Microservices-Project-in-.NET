using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;
[ApiController]
[Route("api/c/platforms")]
public class PlatformController : ControllerBase
{
	private readonly ICommandRepository _repository;
	private readonly IMapper _mapper;

	public PlatformController(ICommandRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpGet]
	public IActionResult GetPlatforms()
	{
		Console.WriteLine("--> Getting Platforms from Command Service....");

		var platforms = _repository.GetAllPlatforms();
		var result = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
		return Ok(result);
	}

	[HttpPost]
	public IActionResult TestInBoundConnection()
	{
		Console.WriteLine("--> Inbound POST # Command Service");
		return Ok("Inbound test of from Platform Cot");
	}
}

