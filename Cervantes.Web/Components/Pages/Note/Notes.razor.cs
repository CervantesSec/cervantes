using System.Net.Http.Json;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Note;

public partial class Notes: ComponentBase
{
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    private string searchString = "";
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    [Inject] private NoteController _NoteController { get; set; }
    private List<CORE.Entities.Note> model = new List<CORE.Entities.Note>();
    private List<CORE.Entities.Note> seleNotes = new List<CORE.Entities.Note>();
    private List<BreadcrumbItem> _items;


    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["notes"], href: null, disabled: true,icon: Icons.Material.Filled.Notes)
        };
        await Update();
        
    }
    
    protected async Task Update()
    {
        model = _NoteController.GetByUserId().ToList();
    }
    
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var dialog = Dialog.Show<CreateNoteDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private Func<CORE.Entities.Note, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.CreatedDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Note> args)
    {
        var parameters = new DialogParameters { ["note"]=args.Item };

        var dialog =  Dialog.Show<NoteDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var records = model.Select(e => new IFR.Export.ClientExport
                {
                    Name = e.Name,
                    Description = e.Description,
                    CreatedDate = e.CreatedDate,
                    ContactName = e.ContactName,
                    ContactPhone = e.ContactPhone,
                    ContactEmail = e.ContactEmail,
                    Url = e.Url
                }).ToList();
                var file = ExportToCsv.ExportClients(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["notes"]=seleNotes };

                var dialog =  Dialog.Show<DeleteNoteBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Note> items)
    {
        
        seleNotes = items.ToList();
    }
 
}