using System.Threading.Tasks;

namespace Cervantes.Web.Helpers;

public interface IAutosaveStorage
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task RemoveAsync(string key);
}

