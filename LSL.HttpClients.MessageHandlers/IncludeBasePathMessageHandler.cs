using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LSL.HttpClients.MessageHandlers
{
    /// <summary>
    /// Inserts a given baePath to the start of the request Uri's Path
    /// </summary>
    public class IncludeBasePathMessageHandler : DelegatingHandler
    {
        private readonly string _basePath;

        /// <summary>
        /// IncludeBasePathMessageHandler constructor
        /// </summary>
        /// <param name="basePath">The base path to insert into a request URI</param>
        public IncludeBasePathMessageHandler(string basePath)
        {
            _basePath = NormaliseBasePath(basePath);
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(request.RequestUri)
            {
                Path = $"{_basePath}{request.RequestUri.AbsolutePath}"
            };

            request.RequestUri = builder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }

        private static string NormaliseBasePath(string basePath)
        {
            string RemoveTrailingSlash(string path) => path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

            if (string.IsNullOrEmpty(basePath)) return basePath;
            if (basePath == "/") return string.Empty;

            return RemoveTrailingSlash(
                basePath.StartsWith("/")
                    ? basePath
                    : $"/{basePath}"
                );
        }
    }
}