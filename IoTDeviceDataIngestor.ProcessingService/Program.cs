var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var rabbitMqSettings = new RabbitMqSettings
{
    QueueName = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? builder.Configuration["RabbitMQ:QueueName"],
    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? builder.Configuration["RabbitMQ:HostName"]
};

builder.Services.AddSingleton(rabbitMqSettings);
builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);
builder.Services.AddSingleton<DataHub>();
builder.Services.AddSingleton<ProcessingService>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventLog();
});
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = rabbitMqSettings.HostName
    };
    return factory;
});

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<DataHub>("/dataHub");
    endpoints.MapControllers();
});

var processingService = app.Services.GetRequiredService<ProcessingService>();
processingService.StartProcessing();
app.Run();
