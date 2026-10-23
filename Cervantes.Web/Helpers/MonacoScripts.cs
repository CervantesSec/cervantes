using Microsoft.JSInterop;

namespace Cervantes.Web.Helpers;

public static class MonacoScripts
{
    /// <summary>
    /// Loads the Monaco editor scripts on first use (wwwroot/lib/monaco-loader.js). False when they fail to load or the
    /// call times out: the dialog then works without the CSS editor instead of ending the circuit.
    /// </summary>
    public static async Task<bool> EnsureAsync(IJSRuntime js)
    {
        try
        {
            await js.InvokeVoidAsync("cervantesMonaco.ensure");
            return true;
        }
        catch (Exception e) when (e is JSException or TaskCanceledException or JSDisconnectedException)
        {
            return false;
        }
    }
}
