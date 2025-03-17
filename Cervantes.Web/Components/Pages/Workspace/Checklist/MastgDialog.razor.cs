using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModel.Mastg;
using Cervantes.CORE.ViewModel.Wstg;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class MastgDialog: ComponentBase
{
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    [Parameter] public Guid project { get; set; }
    [Parameter] public ChecklistViewModel checklist { get; set; }

    [Inject] private ChecklistController _checklistController { get; set; }
    
    MastgViewModel model = new MastgViewModel();
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var mastg = _checklistController.GetMASTGById(checklist.Id);
        model = new MastgViewModel();
        model.Id = checklist.Id;
        model.MobilePlatform = mastg.MobilePlatform;
        model.Storage = new MASTGStorage()
        {
            Storage1Note = mastg.Storage1Note,
            Storage1Status = mastg.Storage1Status,
            Storage1Note1 = mastg.Storage1Note1,
            Storage1Status1 = mastg.Storage1Status1,
            Storage1Note2 = mastg.Storage1Note2,
            Storage1Status2 = mastg.Storage1Status2,
            Storage1Note3 = mastg.Storage1Note3,
            Storage1Status3 = mastg.Storage1Status3,
            Storage2Note = mastg.Storage2Note,
            Storage2Status = mastg.Storage2Status,
            Storage2Note1 = mastg.Storage2Note1,
            Storage2Status1 = mastg.Storage2Status1,
            Storage2Note2 = mastg.Storage2Note2,
            Storage2Status2 = mastg.Storage2Status2,
            Storage2Note3 = mastg.Storage2Note3,
            Storage2Status3 = mastg.Storage2Status3,
            Storage2Note4 = mastg.Storage2Note4,
            Storage2Status4 = mastg.Storage2Status4,
            Storage2Note5 = mastg.Storage2Note5,
            Storage2Status5 = mastg.Storage2Status5,
            Storage2Note6 = mastg.Storage2Note6,
            Storage2Status6 = mastg.Storage2Status6,
            Storage2Note7 = mastg.Storage2Note7,
            Storage2Status7 = mastg.Storage2Status7,
            Storage2Note8 = mastg.Storage2Note8,
            Storage2Status8 = mastg.Storage2Status8,
            Storage2Note9 = mastg.Storage2Note9,
            Storage2Status9 = mastg.Storage2Status9,
            Storage2Note10 = mastg.Storage2Note10,
            Storage2Status10 = mastg.Storage2Status10,
            Storage2Note11 = mastg.Storage2Note11,
            Storage2Status11 = mastg.Storage2Status11
        };
        model.Crypto = new MASTGCrypto()
        {
            Crypto1Note = mastg.Crypto1Note,
            Crypto1Status = mastg.Crypto1Status,
            Crypto1Note1 = mastg.Crypto1Note1,
            Crypto1Status1 = mastg.Crypto1Status1,
            Crypto1Note2 = mastg.Crypto1Note2,
            Crypto1Status2 = mastg.Crypto1Status2,
            Crypto1Note3 = mastg.Crypto1Note3,
            Crypto1Status3 = mastg.Crypto1Status3,
            Crypto1Note4 = mastg.Crypto1Note4,
            Crypto1Status4 = mastg.Crypto1Status4,
            Crypto1Note5 = mastg.Crypto1Note5,
            Crypto1Status5 = mastg.Crypto1Status5,
            Crypto2Note = mastg.Crypto2Note,
            Crypto2Status = mastg.Crypto2Status,
            Crypto2Note1 = mastg.Crypto2Note1,
            Crypto2Status1 = mastg.Crypto2Status1,
            Crypto2Note2 = mastg.Crypto2Note2,
            Crypto2Status2 = mastg.Crypto2Status2
        };
        model.Auth = new MASTGAuth()
        {
            Auth1Note = mastg.Auth1Note,
            Auth1Status = mastg.Auth1Status,
            Auth2Note = mastg.Auth2Note,
            Auth2Status = mastg.Auth2Status,
            Auth2Note1 = mastg.Auth2Note1,
            Auth2Status1 = mastg.Auth2Status1,
            Auth2Note2 = mastg.Auth2Note2,
            Auth2Status2 = mastg.Auth2Status2,
            Auth2Note3 = mastg.Auth2Note3,
            Auth2Status3 = mastg.Auth2Status3,
            Auth3Note = mastg.Auth3Note,
            Auth3Status = mastg.Auth3Status,
            
        };
        model.Network = new MASTGNetwork()
        {
            Network1Note = mastg.Network1Note,
            Network1Status = mastg.Network1Status,
            Network1Note1 = mastg.Network1Note1,
            Network1Status1 = mastg.Network1Status1,
            Network1Note2 = mastg.Network1Note2,
            Network1Status2 = mastg.Network1Status2,
            Network1Note3 = mastg.Network1Note3,
            Network1Status3 = mastg.Network1Status3,
            Network1Note4 = mastg.Network1Note4,
            Network1Status4 = mastg.Network1Status4,
            Network1Note5 = mastg.Network1Note5,
            Network1Status5 = mastg.Network1Status5,
            Network1Note6 = mastg.Network1Note6,
            Network1Status6 = mastg.Network1Status6,
            Network1Note7 = mastg.Network1Note7,
            Network1Status7 = mastg.Network1Status7,
            Network2Note = mastg.Storage2Note,
            Network2Status = mastg.Storage2Status,
            Network2Note1 = mastg.Storage2Note1,
            Network2Status1 = mastg.Storage2Status1,
            Network2Note2 = mastg.Storage2Note2,
            Network2Status2 = mastg.Storage2Status2,
        };

        model.Platform = new MASTGPlatform()
        {
            Platform1Note = mastg.Platform1Note,
            Platform1Status = mastg.Platform1Status,
            Platform1Note1 = mastg.Platform1Note1,
            Platform1Status1 = mastg.Platform1Status1,
            Platform1Note2 = mastg.Platform1Note2,
            Platform1Status2 = mastg.Platform1Status2,
            Platform1Note3 = mastg.Platform1Note3,
            Platform1Status3 = mastg.Platform1Status3,
            Platform1Note4 = mastg.Platform1Note4,
            Platform1Status4 = mastg.Platform1Status4,
            Platform1Note5 = mastg.Platform1Note5,
            Platform1Status5 = mastg.Platform1Status5,
            Platform1Note6 = mastg.Platform1Note6,
            Platform1Status6 = mastg.Platform1Status6,
            Platform1Note7 = mastg.Platform1Note7,
            Platform1Status7 = mastg.Platform1Status7,
            Platform1Note8 = mastg.Platform1Note8,
            Platform1Status8 = mastg.Platform1Status8,
            Platform1Note9 = mastg.Platform1Note9,
            Platform1Status9 = mastg.Platform1Status9,
            Platform1Note10 = mastg.Platform1Note10,
            Platform1Status10 = mastg.Platform1Status10,
            Platform1Note11 = mastg.Platform1Note11,
            Platform1Status11 = mastg.Platform1Status11,
            Platform1Note12 = mastg.Platform1Note12,
            Platform1Status12 = mastg.Platform1Status12,
            Platform1Note13 = mastg.Platform1Note13,
            Platform1Status13 = mastg.Platform1Status13,
            Platform2Note = mastg.Platform2Note,
            Platform2Status = mastg.Platform2Status,
            Platform2Note1 = mastg.Platform2Note1,
            Platform2Status1 = mastg.Platform2Status1,
            Platform2Note2 = mastg.Platform2Note2,
            Platform2Status2 = mastg.Platform2Status2,
            Platform2Note3 = mastg.Platform2Note3,
            Platform2Status3 = mastg.Platform2Status3,
            Platform2Note4 = mastg.Platform2Note4,
            Platform2Status4 = mastg.Platform2Status4,
            Platform2Note5 = mastg.Platform2Note5,
            Platform2Status5 = mastg.Platform2Status5,
            Platform2Note6 = mastg.Platform2Note6,
            Platform2Status6 = mastg.Platform2Status6,
            Platform2Note7 = mastg.Platform2Note7,
            Platform2Status7 = mastg.Platform2Status7,
            Platform3Note = mastg.Platform3Note,
            Platform3Status = mastg.Platform3Status,
            Platform3Note1 = mastg.Platform3Note1,
            Platform3Status1 = mastg.Platform3Status1,
            Platform3Note2 = mastg.Platform3Note2,
            Platform3Status2 = mastg.Platform3Status2,
            Platform3Note3 = mastg.Platform3Note3,
            Platform3Status3 = mastg.Platform3Status3,
            Platform3Note4 = mastg.Platform3Note4,
            Platform3Status4 = mastg.Platform3Status4,
            Platform3Note5 = mastg.Platform3Note5,
            Platform3Status5 = mastg.Platform3Status5,
        };

        model.Code = new MASTGCode()
        {
            Code1Note = mastg.Code1Note,
            Code1Status = mastg.Code1Status,
            Code2Note = mastg.Code2Note,
            Code2Status = mastg.Code2Status,
            Code2Note1 = mastg.Code2Note1,
            Code2Status1 = mastg.Code2Status1,
            Code2Note2 = mastg.Code2Note2,
            Code2Status2 = mastg.Code2Status2,
            Code3Note = mastg.Code3Note,
            Code3Status = mastg.Code3Status,
            Code3Note1 = mastg.Code3Note1,
            Code3Status1 = mastg.Code3Status1,
            Code3Note2 = mastg.Code3Note2,
            Code3Status2 = mastg.Code3Status2,
            Code4Note = mastg.Code4Note,
            Code4Status = mastg.Code4Status,
            Code4Note1 = mastg.Code4Note1,
            Code4Status1 = mastg.Code4Status1,
            Code4Note2 = mastg.Code4Note2,
            Code4Status2 = mastg.Code4Status2,
            Code4Note3 = mastg.Code4Note3,
            Code4Status3 = mastg.Code4Status3,
            Code4Note4 = mastg.Code4Note4,
            Code4Status4 = mastg.Code4Status4,
            Code4Note5 = mastg.Code4Note5,
            Code4Status5 = mastg.Code4Status5,
            Code4Note6 = mastg.Code4Note6,
            Code4Status6 = mastg.Code4Status6,
            Code4Note7 = mastg.Code4Note7,
            Code4Status7 = mastg.Code4Status7,
            Code4Note8 = mastg.Code4Note8,
            Code4Status8 = mastg.Code4Status8,
            Code4Note9 = mastg.Code4Note9,
            Code4Status9 = mastg.Code4Status9,
            Code4Note10 = mastg.Code4Note10,
            Code4Status10 = mastg.Code4Status10,

        };

        model.Resilience = new MASTGResilience()
        {
            Resilience1Note = mastg.Resilience1Note,
            Resilience1Status = mastg.Resilience1Status,    
            Resilience1Note1 = mastg.Resilience1Note1,
            Resilience1Status1 = mastg.Resilience1Status1,
            Resilience1Note2 = mastg.Resilience1Note2,
            Resilience1Status2 = mastg.Resilience1Status2,
            Resilience1Note3 = mastg.Resilience1Note3,
            Resilience1Status3 = mastg.Resilience1Status3,
            Resilience1Note4 = mastg.Resilience1Note4,
            Resilience1Status4 = mastg.Resilience1Status4,
            Resilience2Note = mastg.Resilience2Note,
            Resilience2Status = mastg.Resilience2Status,
            Resilience2Note1 = mastg.Resilience2Note1,
            Resilience2Status1 = mastg.Resilience2Status1,
            Resilience2Note2 = mastg.Resilience2Note2,
            Resilience2Status2 = mastg.Resilience2Status2,
            Resilience2Note3 = mastg.Resilience2Note3,
            Resilience2Status3 = mastg.Resilience2Status3,
            Resilience2Note4 = mastg.Resilience2Note4,
            Resilience2Status4 = mastg.Resilience2Status4,
            Resilience2Note5 = mastg.Resilience2Note5,
            Resilience2Status5 = mastg.Resilience2Status5,
            Resilience3Note = mastg.Resilience3Note,
            Resilience3Status = mastg.Resilience3Status,
            Resilience3Note1 = mastg.Resilience3Note1,
            Resilience3Status1 = mastg.Resilience3Status1,
            Resilience3Note2 = mastg.Resilience3Note2,
            Resilience3Status2 = mastg.Resilience3Status2,
            Resilience3Note3 = mastg.Resilience3Note3,
            Resilience3Status3 = mastg.Resilience3Status3,
            Resilience3Note4 = mastg.Resilience3Note4,
            Resilience3Status4 = mastg.Resilience3Status4,
            Resilience3Note5 = mastg.Resilience3Note5,
            Resilience3Status5 = mastg.Resilience3Status5,
            Resilience3Note6 = mastg.Resilience3Note6,
            Resilience3Status6 = mastg.Resilience3Status6,
            Resilience4Note = mastg.Resilience4Note,
            Resilience4Status = mastg.Resilience4Status,
            Resilience4Note1 = mastg.Resilience4Note1,
            Resilience4Status1 = mastg.Resilience4Status1,
            Resilience4Note2 = mastg.Resilience4Note2,
            Resilience4Status2 = mastg.Resilience4Status2,
            Resilience4Note3 = mastg.Resilience4Note3,
            Resilience4Status3 = mastg.Resilience4Status3,
            Resilience4Note4 = mastg.Resilience4Note4,
            Resilience4Status4 = mastg.Resilience4Status4,
            Resilience4Note5 = mastg.Resilience4Note5,
            Resilience4Status5 = mastg.Resilience4Status5,
            Resilience4Note6 = mastg.Resilience4Note6,
            Resilience4Status6 = mastg.Resilience4Status6,
        };

            StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.Id = checklist.Id;
            var response = await _checklistController.EditMastg(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["checklistEdited"], Severity.Success);
                //MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["checklistEditedError"], Severity.Error);
            }
            
        }
    }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    
    private async Task GenerateReport()
    {
        var parameters = new DialogParameters { ["checklistType"]=checklist.Type.ToString(),["checklistId"]=checklist.Id,["project"]=project };
        var dialog2 =  DialogService.Show<CreateReportChecklistDialog>(@localizer["delete"],parameters,maxWidth);
        var result2 = await dialog2.Result;

        if (!result2.Canceled)
        {
            NavigationManager.NavigateTo($"/workspace/{project}/details");               
        }
    }
    
    async Task DeleteDialog(ChecklistViewModel list,DialogOptions options)
    {
        var parameters = new DialogParameters { ["checklist"]=list };
        
        switch (list.Type)
        {
            case ChecklistType.OWASPWSTG:
                var dialog =  DialogService.Show<DeleteChecklistDialog>(@localizer["delete"], parameters,options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    MudDialog.Close(DialogResult.Ok(true));                }
                break;
            case ChecklistType.OWASPMASVS:
                var dialog2 =  DialogService.Show<DeleteChecklistDialog>(@localizer["delete"], parameters,options);
                var result2 = await dialog2.Result;

                if (!result2.Canceled)
                {
                    MudDialog.Close(DialogResult.Ok(true));                }
                break;
        }
        
    }
}