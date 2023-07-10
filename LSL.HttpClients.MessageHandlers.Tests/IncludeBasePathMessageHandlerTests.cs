using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LSL.HttpClients.MessageHandlers.Tests
{
    public class IncludeBasePathMessageHandlerTests
    {
        [TestCase(null, "/v1/example")]
        [TestCase("", "/v1/example")]
        [TestCase("/", "/v1/example")]
        [TestCase("/base", "/base/v1/example")]
        [TestCase("/base/", "/base/v1/example")]
        [TestCase("base/", "/base/v1/example")]
        [TestCase("/base/and-more", "/base/and-more/v1/example")]
        [TestCase("/base/and-more/", "/base/and-more/v1/example")]
        public async Task GivenABasePath_ItShouldSendMessagesUsingTheExpectedPath(string basePath, string expectedAbsolutePath)
        {   
            var capturedAbsolutePath = string.Empty;

            using var httpClient = new HttpClient(new IncludeBasePathMessageHandler(basePath)
            {
                InnerHandler = new DelegateAcceptingHandler(r => capturedAbsolutePath = r.RequestUri.AbsolutePath)
            });

            await httpClient.GetAsync("http://my-service.com/v1/example");

            capturedAbsolutePath.Should().Be(expectedAbsolutePath);
        }
        
        private class DelegateAcceptingHandler : DelegatingHandler
        {
            private readonly Action<HttpRequestMessage> _actionToRun;

            public DelegateAcceptingHandler(Action<HttpRequestMessage> actionToRun)
            {
                _actionToRun = actionToRun;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _actionToRun(request);
                
                return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage());
            }
        }
    }
}
