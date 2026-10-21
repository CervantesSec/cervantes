using Cervantes.Web.AuthPermissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cervantes.Tests.Web.AuthPermissions;

public class BasicAuthMiddlewareTests
{
    private sealed class StartedResponseFeature : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }

    private static BasicAuthMiddleware CreateMiddleware() =>
        new(_ => throw new InvalidOperationException("boom"),
            NullLogger<BasicAuthMiddleware>.Instance,
            null!);

    [Fact]
    public async Task Invoke_WhenNextThrowsAfterTheResponseStarted_Rethrows()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/not-api";
        context.Features.Set<IHttpResponseFeature>(new StartedResponseFeature());

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateMiddleware().InvokeAsync(context));
    }

    [Fact]
    public async Task Invoke_WhenNextThrowsBeforeTheResponseStarted_Returns500()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/not-api";
        var body = new MemoryStream();
        context.Response.Body = body;

        await CreateMiddleware().InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
        body.Position = 0;
        Assert.Equal("An unexpected error occurred.", new StreamReader(body).ReadToEnd());
    }
}
