namespace IoTDeviceDataIngestor.ProcessingService.Utilities
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
            => _logger = logger;

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(
                context.Exception,
                $"A {context.Exception.GetType()} exception has occurred.",
                context.Exception.Message,
                context.Exception.InnerException,
                context.Exception.StackTrace);
            context.Result = new ObjectResult(StatusCodes.Status500InternalServerError);
        }
    }
}
