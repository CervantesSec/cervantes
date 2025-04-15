using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace Cervantes.Web.Components.Layout
{

    public partial class CultureSelector : ComponentBase
    {
        private CultureInfo[] supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("es-ES"),
            new CultureInfo("pt-PT"),
            new CultureInfo("tr-TR"),
        };

        protected async override Task OnInitializedAsync()
        {

            Culture = CultureInfo.CurrentCulture;

            await base.OnInitializedAsync();
        }

        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var uri = new Uri(NavigationManager.Uri)
                        .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                    var cultureEscaped = Uri.EscapeDataString(value.Name);
                    var uriEscaped = Uri.EscapeDataString(uri);

                    NavigationManager.NavigateTo(
                        $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
                        forceLoad: true);
                }
            }
        }

        public void CultureSet(CultureInfo culture)
        {
            Culture = culture;
        }
    }
}

