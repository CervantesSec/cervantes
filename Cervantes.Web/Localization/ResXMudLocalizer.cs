using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Cervantes.Web.Localization;

public class ResXMudLocalizer: MudLocalizer
{
    private IStringLocalizer _localization;

    public ResXMudLocalizer(IStringLocalizer<MudResources> localizer)
    {
        _localization = localizer;
    }

    public override LocalizedString this[string key] => _localization[key];
}