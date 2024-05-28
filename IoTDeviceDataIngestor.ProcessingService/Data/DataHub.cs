namespace IoTDeviceDataIngestor.ProcessingService.Data
{
    public class DataHub : Hub
    {
        public async Task SendChunkedMessages(IEnumerable<JToken> messages)
        {
            var chunks = Chunkify(messages, 10);
            foreach (var chunk in chunks)
                await Task.Run(() => SendChunkToClients(chunk));
        }

        private IEnumerable<IEnumerable<JToken>> Chunkify(IEnumerable<JToken> messages, int chunkSize)
        {
            for (int i = 0; i < messages.Count(); i += chunkSize)
            {
                yield return messages.Skip(i).Take(chunkSize);
            }
        }

        private async Task SendChunkToClients(IEnumerable<JToken> chunk)
        {
            foreach (var message in chunk)
                await Clients.All.SendAsync("ReceiveMessage", message.ToString());
        }
    }
}
