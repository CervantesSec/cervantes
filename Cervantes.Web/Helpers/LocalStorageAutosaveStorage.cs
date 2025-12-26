using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Cervantes.Web.Helpers;

public class LocalStorageAutosaveStorage(IJSRuntime js) : IAutosaveStorage
{
    private readonly IJSRuntime _js = js;

    public async Task<string?> GetAsync(string key)
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", key);
    }

    public async Task SetAsync(string key, string value)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public async Task RemoveAsync(string key)
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", key);
    }
}

