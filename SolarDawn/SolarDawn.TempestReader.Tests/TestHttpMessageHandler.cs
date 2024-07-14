using System.Net;

namespace SolarDawn.TempestReader.Tests
{
    internal class TestHttpMessageHandler : HttpMessageHandler
    {
        private int _current;
        public readonly List<HttpResponseMessage> Responses = [];
        public readonly List<HttpRequestMessage> Requests = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            if (_current > Responses.Count)
            {
                return Task.FromResult(Responses[_current++]);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
        }
    }
}
