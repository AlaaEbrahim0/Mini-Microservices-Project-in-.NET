using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SynDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
	.AddEndpointsApiExplorer()
	.AddSwaggerGen()
	.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
	.AddSingleton<IEventProcessor, EventProcessor>()
	.AddScoped<ICommandRepository, CommandRepository>()
	.AddScoped<IPlatformDataClient, PlatformDataClient>()
	.AddHostedService<MessageBusSubscriber>()
	.AddDbContext<AppDbContext>(options =>
	{
		options.UseInMemoryDatabase("InMemory");
	});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.PrepPopulation();

app.Run();
