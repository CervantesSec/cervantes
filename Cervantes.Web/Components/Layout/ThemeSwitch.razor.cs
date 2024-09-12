using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Layout;

public partial class ThemeSwitch : ComponentBase
{
    [Parameter] public bool isDarkMode { get; set; }
    [Parameter] public EventCallback<bool> isDarkModeChanged { get; set; }
    private async Task OnIsDarkModeChanged(bool value)
        => await this.isDarkModeChanged.InvokeAsync(value);
    
    private bool SelectedVector
    {
        get => isDarkMode;
        set
        {
            if (value != isDarkMode)
            {
                isDarkModeChanged.InvokeAsync(value);        
            }
        }
    }
    
    private async Task ChangeTheme()
    {
        isDarkMode = !isDarkMode;
        await OnIsDarkModeChanged(isDarkMode);
        string theme = await LocalStorage.GetItemAsStringAsync("theme");

        if (theme == "dark")
        {
            await LocalStorage.SetItemAsStringAsync("theme", "light");
        }
        else
        {
            await LocalStorage.SetItemAsStringAsync("theme", "dark");
        }
    }
}