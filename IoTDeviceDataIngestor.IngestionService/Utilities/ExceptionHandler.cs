namespace IoTDeviceDataIngestor.IngestionService.Utilities
{
    public static class ExceptionHandler
    {
        public static IActionResult HandleException(Exception ex)
            => ex switch
            {
                ArgumentNullException => new ObjectResult("Invalid request") { StatusCode = StatusCodes.Status400BadRequest },
                ArgumentException => new ObjectResult("Invalid value format in request") { StatusCode = StatusCodes.Status400BadRequest },
                ValidationException => new ObjectResult("Invalid request") { StatusCode = StatusCodes.Status400BadRequest },
                UnauthorizedAccessException => new ObjectResult("Access denied") { StatusCode = StatusCodes.Status401Unauthorized },
                InvalidOperationException => new ObjectResult("Operation not implemented") { StatusCode = StatusCodes.Status501NotImplemented },
                TimeoutException => new ObjectResult("Timeout from gateway") { StatusCode = StatusCodes.Status504GatewayTimeout },
                _ => new ObjectResult("An internal server error occurred") { StatusCode = StatusCodes.Status500InternalServerError }
            };
    }
}
