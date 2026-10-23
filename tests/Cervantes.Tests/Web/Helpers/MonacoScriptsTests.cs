using Cervantes.Web.Helpers;
using Microsoft.JSInterop;

namespace Cervantes.Tests.Web.Helpers;

public class MonacoScriptsTests
{
    private sealed class FakeJs : IJSRuntime
    {
        private readonly Exception? error;
        public string? Called { get; private set; }

        public FakeJs(Exception? error = null) => this.error = error;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Called = identifier;
            return error == null ? ValueTask.FromResult(default(TValue)!) : ValueTask.FromException<TValue>(error);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }

    [Fact]
    public async Task EnsureAsync_LoadedReturnsTrue()
    {
        var js = new FakeJs();

        Assert.True(await MonacoScripts.EnsureAsync(js));
        Assert.Equal("cervantesMonaco.ensure", js.Called);
    }

    // A failed or slow load must leave the dialog usable (without the CSS editor), never end the circuit.
    [Fact]
    public async Task EnsureAsync_ScriptErrorReturnsFalse()
    {
        Assert.False(await MonacoScripts.EnsureAsync(new FakeJs(new JSException("Could not load loader.js"))));
    }

    [Fact]
    public async Task EnsureAsync_InteropTimeoutReturnsFalse()
    {
        Assert.False(await MonacoScripts.EnsureAsync(new FakeJs(new TaskCanceledException())));
    }
}
