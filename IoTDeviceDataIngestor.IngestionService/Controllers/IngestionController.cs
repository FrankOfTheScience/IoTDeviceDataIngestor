using IoTDeviceDataIngestor.IngestionService.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Text;

namespace IoTDeviceDataIngestor.IngestionService.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class IngestionController : ControllerBase
    {
        private readonly ILogger<IngestionController> _logger;
        private readonly IModel _channel;
        private readonly string _queueName;

        public IngestionController(ILogger<IngestionController> logger, IModel channel, RabbitMqSettings rabbitSettings)
        {
            _logger = logger;
            _channel = channel;
            _queueName = rabbitSettings.QueueName;
        }

        [HttpPost(Name = "IngestData")]
        public IActionResult IngestDataAsync([FromBody] JArray dataArray)
        {
            if (dataArray is null)
            {
                _logger.LogInformation("Payload is empty");
                return Ok("No message has been sent");
            }
            _logger.LogInformation("Received data: {@Data}", dataArray);

            _channel.QueueDeclare(queue: _queueName,
                                  durable: true,
                                  exclusive: true,
                                  autoDelete: true,
                                  arguments: null);

            var message = JsonConvert.SerializeObject(dataArray);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: _queueName,
                                  basicProperties: null,
                                  body: body);

            _logger.LogInformation($"Message published to queue {_queueName}");
            return Ok("Message sent");
        }
    }
}
