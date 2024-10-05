using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
	.AddEndpointsApiExplorer()
	.AddSwaggerGen()
	.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
	.AddSingleton<IEventProcessor, EventProcessor>()
	.AddScoped<ICommandRepository, CommandRepository>()
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

app.Run();
