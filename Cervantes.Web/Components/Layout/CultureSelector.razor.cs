using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace Cervantes.Web.Components.Layout
{
    
    public partial class CultureSelector
    {
        NavigationManager Navigation;
        
        private CultureInfo[] supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("es-ES"),
        };

        protected override void OnInitialized()
        {
            Culture = CultureInfo.CurrentCulture;
        }

        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var uri = new Uri(Navigation.Uri)
                        .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                    var cultureEscaped = Uri.EscapeDataString(value.Name);
                    var uriEscaped = Uri.EscapeDataString(uri);

                    Navigation.NavigateTo(
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

