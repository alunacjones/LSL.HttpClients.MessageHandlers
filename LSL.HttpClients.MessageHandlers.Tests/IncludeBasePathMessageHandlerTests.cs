using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;

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

        [Test]
        public void Env()
        {
            var vars = Environment.GetEnvironmentVariables().OfType<DictionaryEntry>()
                .ToDictionary(de => de.Key, de => de.Value)
                .OrderBy(kvp => kvp.Key);

            File.WriteAllText("./myvars.json", JsonConvert.SerializeObject(vars, Formatting.Indented));
        }

        [Test]
        public void Other()
        {
            var u = new Uri("message:exchange/topic?queue=asd&routeToDev=1");
            var qs = System.Web.HttpUtility.ParseQueryString(u.Query);
            qs["another"] = "oh!#@$/:";
            var ub = new UriBuilder(u);            
            ub.Query = qs.ToString();
            var r = Uri.EscapeDataString("my/queue");
            ub.Path = $"{r}/asdasd";

            Convert.FromBase64String("asd'");
        }
    }    
}
