using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IDisposable
{
	private readonly IConfiguration _configuration;
	private readonly IConnection _connection;

	public MessageBusClient(IConfiguration configuration)
	{
		_configuration = configuration;
		var factory = new ConnectionFactory()
		{
			HostName = _configuration["RabbitMQHost"],
			Port = int.Parse(_configuration["RabbitMQPort"]!),
		};

		try
		{
			_connection = factory.CreateConnection();
			_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

			Console.WriteLine("--> Connected to MessageBus");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Could not connect to message bus: {ex.Message}");
		}
	}

	public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
	{
		var message = JsonSerializer.Serialize(platformPublishedDto);

		if (!_connection.IsOpen)
		{
			Console.WriteLine("--> RabbitMQ Connection closed, not sending");
			return;
		}

		using (var channel = _connection.CreateModel())
		{
			channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
			var body = Encoding.UTF8.GetBytes(message);

			channel.BasicPublish(
				exchange: "trigger",
				routingKey: "",
				basicProperties: null,
				body: body);

			Console.WriteLine($"We sent {message}");
		}

		Console.WriteLine($"--> RabbitMQ Connection open, sent {message}");
	}

	public void Dispose()
	{
		Console.WriteLine("MessageBus Disposed");
		if (_connection.IsOpen)
		{
			_connection.Close();
		}
	}

	private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
	{
		Console.WriteLine("--> RabbitMQ Connection Shutdown");
	}
}
