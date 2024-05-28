namespace IoTDeviceDataIngestor.ProcessingService.Services
{
    public class ProcessingService
    {
        private readonly ILogger<ProcessingService> _logger;
        private readonly DataHub _hubContext;
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queueName;

        public ProcessingService(ILogger<ProcessingService> logger, IConnectionFactory connectionFactory, DataHub hubContext, RabbitMqSettings rabbitSettings)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _hubContext = hubContext;
            _queueName = rabbitSettings.QueueName;
        }

        public void StartProcessing()
        {
            Task.Run(StartConsuming);
        }

        private async Task StartConsuming()
        {
            _logger.LogInformation($"Starting consuming messages at: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: true,
                                     autoDelete: true,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var jsonArray = JArray.Parse(message);

                        await _hubContext.SendChunkedMessages(jsonArray);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                                         $"An exception occurred while processing a message: {ex.Message}",
                                         ex.StackTrace);
                    }
                };

                channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }

            _logger.LogInformation($"Ended consuming messages at: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        }
    }
}
