[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-httpclients-messagehandlers.svg)](https://ci.appveyor.com/project/alunacjones/lsl-httpclients-messagehandlers)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.HttpClients.MessageHandlers)](https://coveralls.io/github/alunacjones/LSL.HttpClients.MessageHandlers)
[![NuGet](https://img.shields.io/nuget/v/LSL.HttpClients.MessageHandlers.svg)](https://www.nuget.org/packages/LSL.HttpClients.MessageHandlers/)

# LSL.HttpClients.MessageHandlers

This library currently provides the following message handlers to be used with `HttpClient` instances:

* IncludeBasePathMessageHandler

## IncludeBasePathMessageHandler

This message handler is useful for clients that use relative paths to end-points but may not be hosted on a base url with no path.

This message handler must be constructed with a `basePath` parameter. The `basePath` is then injected into the requests `URI`'s path portion, at the start.
Trailing slashes are accommodated for and normalised.

### Example using AddHttpClient

```csharp
using LSL.HttpClients.MessageHandlers;
....

// Within your host builder service registration (assuming a serviceCollection variable is already defined)
serviceCollection.AddHttpClient<IYourClient, YourClient>()
    .AddHttpMessageHandler(() => new IncludeBasePathMessageHandler("my-base-path"))

// Any calls using IYourClient (assuming a HttpClient BasePath of https://example.com) would ensure the actual base
// path starts with https://example.com/my-base-path/
// NOTE: the base path is not restricted to a single path segment either e.g. "my-base-path" in the code above
// could have a further path portion e.g. "my-base-path/v1"
