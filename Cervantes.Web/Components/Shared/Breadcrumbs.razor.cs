using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Shared;

public partial class Breadcrumbs: ComponentBase
{
    [CascadingParameter] 
    private bool _isDarkMode { get; set; }
    
    /// <summary>
    /// Lista de elementos de navegación para los breadcrumbs
    /// </summary>
    [Parameter]
    public List<BreadcrumbItem> Items { get; set; } = new List<BreadcrumbItem>();
    
    /// <summary>
    /// Carácter separador entre los elementos de navegación
    /// </summary>
    [Parameter]
    public string Separator { get; set; } = ">";
    
    private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
    
    /// <summary>
    /// Inicializa el componente copiando los items al campo privado
    /// </summary>
    protected override void OnInitialized()
    {
        UpdateItems();
    }
    
    /// <summary>
    /// Actualiza los items cuando cambian los parámetros
    /// </summary>
    protected override void OnParametersSet()
    {
        UpdateItems();
    }
    
    /// <summary>
    /// Actualiza la lista interna de items
    /// </summary>
    private void UpdateItems()
    {
        if (Items != null)
        {
            _items = Items.ToList();
        }
    }

}