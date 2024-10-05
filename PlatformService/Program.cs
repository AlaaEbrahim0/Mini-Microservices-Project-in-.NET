using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddGrpc();

if (builder.Environment.IsProduction())
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformServiceDbConnectionString")));
}
else
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseInMemoryDatabase("InMemoryDatabase"));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

Console.WriteLine(builder.Configuration["CommandServiceUrl"]);

app.MapControllers();

app.MapGrpcService<GrpcPlatformService>();

app.PrepPopulation(builder.Environment.IsProduction());

app.Run();

var server = new KestrelServer(
	Options.Create(new KestrelServerOptions()),
	new SocketTransportFactory(
		Options.Create(new SocketTransportOptions()),
		new NullLoggerFactory()),
	new NullLoggerFactory()
	);