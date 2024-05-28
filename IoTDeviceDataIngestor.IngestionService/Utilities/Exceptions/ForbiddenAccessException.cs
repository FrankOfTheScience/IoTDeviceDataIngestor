namespace IoTDeviceDataIngestor.IngestionService.Utilities.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string? message, Exception? innerException) : base(message, innerException)
        { }
    }
}
