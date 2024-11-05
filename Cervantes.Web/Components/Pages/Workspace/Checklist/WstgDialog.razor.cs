using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModel.Wstg;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class WstgDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }

    MudForm form;
    [Parameter] public Guid project { get; set; }
    [Parameter] public ChecklistViewModel checklist { get; set; }
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    
    WSTGViewModel model = new WSTGViewModel();
    ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var wstg = _checklistController.GetWSTGById(checklist.Id);
        model = new WSTGViewModel();

           model.Info = new WSTGInfo
            {
                Info01Note = wstg.Info01Note,
                Info01Status = wstg.Info01Status,
                Info02Note = wstg.Info02Note,
                Info02Status = wstg.Info02Status,
                Info03Note = wstg.Info03Note,
                Info03Status = wstg.Info03Status,
                Info04Note = wstg.Info04Note,
                Info04Status = wstg.Info04Status,
                Info05Note = wstg.Info05Note,
                Info05Status = wstg.Info05Status,
                Info06Note = wstg.Info06Note,
                Info06Status = wstg.Info06Status,
                Info07Note = wstg.Info07Note,
                Info07Status = wstg.Info07Status,
                Info08Note = wstg.Info08Note,
                Info08Status = wstg.Info08Status,
                Info09Note = wstg.Info09Note,
                Info09Status = wstg.Info09Status,
                Info10Note = wstg.Info10Note,
                Info10Status = wstg.Info10Status,
            };
            model.Apit = new WSTGApit
            {
                Apit01Note = wstg.Apit01Note,
                Apit01Status = wstg.Apit01Status
            };
            model.Athz = new WSTGAthz
            {
                Athz01Note = wstg.Athz01Note,
                Athz02Note = wstg.Athz02Note,
                Athz03Note = wstg.Athz03Note,
                Athz04Note = wstg.Athz04Note,
                Athz01Status = wstg.Athz01Status,
                Athz02Status = wstg.Athz02Status,
                Athz03Status = wstg.Athz03Status,
                Athz04Status = wstg.Athz04Status
            };
            model.Auth = new WSTGAuth
            {
                Athn01Note = wstg.Athn01Note,
                Athn02Note = wstg.Athn02Note,
                Athn03Note = wstg.Athn03Note,
                Athn04Note = wstg.Athn04Note,
                Athn05Note = wstg.Athn05Note,
                Athn06Note = wstg.Athn06Note,
                Athn07Note = wstg.Athn07Note,
                Athn08Note = wstg.Athn08Note,
                Athn09Note = wstg.Athn09Note,
                Athn10Note = wstg.Athn10Note,
                Athn01Status = wstg.Athn01Status,
                Athn02Status = wstg.Athn02Status,
                Athn03Status = wstg.Athn03Status,
                Athn04Status = wstg.Athn04Status,
                Athn05Status = wstg.Athn05Status,
                Athn06Status = wstg.Athn06Status,
                Athn07Status = wstg.Athn07Status,
                Athn08Status = wstg.Athn08Status,
                Athn09Status = wstg.Athn09Status,
                Athn10Status = wstg.Athn10Status
            };
            model.Busl = new WSTGBusl
            {
                Busl01Note = wstg.Busl01Note,
                Busl02Note = wstg.Busl02Note,
                Busl03Note = wstg.Busl03Note,
                Busl04Note = wstg.Busl04Note,
                Busl05Note = wstg.Busl05Note,
                Busl06Note = wstg.Busl06Note,
                Busl07Note = wstg.Busl07Note,
                Busl08Note = wstg.Busl08Note,
                Busl09Note = wstg.Busl09Note,
                Busl01Status = wstg.Busl01Status,
                Busl02Status = wstg.Busl02Status,
                Busl03Status = wstg.Busl03Status,
                Busl04Status = wstg.Busl04Status,
                Busl05Status = wstg.Busl05Status,
                Busl06Status = wstg.Busl06Status,
                Busl07Status = wstg.Busl07Status,
                Busl08Status = wstg.Busl08Status,
                Busl09Status = wstg.Busl09Status
            };
            model.Clnt = new WSTGClnt
            {
                Clnt01Note = wstg.Clnt01Note,
                Clnt02Note = wstg.Clnt02Note,
                Clnt03Note = wstg.Clnt03Note,
                Clnt04Note = wstg.Clnt04Note,
                Clnt05Note = wstg.Clnt05Note,
                Clnt06Note = wstg.Clnt06Note,
                Clnt07Note = wstg.Clnt07Note,
                Clnt08Note = wstg.Clnt08Note,
                Clnt09Note = wstg.Clnt09Note,
                Clnt10Note = wstg.Clnt10Note,
                Clnt11Note = wstg.Clnt11Note,
                Clnt12Note = wstg.Clnt12Note,
                Clnt13Note = wstg.Clnt13Note,
                Clnt01Status = wstg.Clnt01Status,
                Clnt02Status = wstg.Clnt02Status,
                Clnt03Status = wstg.Clnt03Status,
                Clnt04Status = wstg.Clnt04Status,
                Clnt05Status = wstg.Clnt05Status,
                Clnt06Status = wstg.Clnt06Status,
                Clnt07Status = wstg.Clnt07Status,
                Clnt08Status = wstg.Clnt08Status,
                Clnt09Status = wstg.Clnt09Status,
                Clnt10Status = wstg.Clnt10Status,
                Clnt11Status = wstg.Clnt11Status,
                Clnt12Status = wstg.Clnt12Status,
                Clnt13Status = wstg.Clnt13Status
            };
            model.Conf = new WSTGConf
            {
                Conf01Note = wstg.Conf01Note,
                Conf02Note = wstg.Conf02Note,
                Conf03Note = wstg.Conf03Note,
                Conf04Note = wstg.Conf04Note,
                Conf05Note = wstg.Conf05Note,
                Conf06Note = wstg.Conf06Note,
                Conf07Note = wstg.Conf07Note,
                Conf08Note = wstg.Conf08Note,
                Conf09Note = wstg.Conf09Note,
                Conf10Note = wstg.Conf10Note,
                Conf11Note = wstg.Conf11Note,
                Conf01Status = wstg.Conf01Status,
                Conf02Status = wstg.Conf02Status,
                Conf03Status = wstg.Conf03Status,
                Conf04Status = wstg.Conf04Status,
                Conf05Status = wstg.Conf05Status,
                Conf06Status = wstg.Conf06Status,
                Conf07Status = wstg.Conf07Status,
                Conf08Status = wstg.Conf08Status,
                Conf09Status = wstg.Conf09Status,
                Conf10Status = wstg.Conf10Status,
                Conf11Status = wstg.Conf11Status
            };
            model.Cryp = new WSTGCryp
            {
                Cryp01Note = wstg.Cryp01Note,
                Cryp02Note = wstg.Cryp02Note,
                Cryp03Note = wstg.Cryp03Note,
                Cryp04Note = wstg.Cryp04Note,
                Cryp01Status = wstg.Cryp01Status,
                Cryp02Status = wstg.Cryp02Status,
                Cryp03Status = wstg.Cryp03Status,
                Cryp04Status = wstg.Cryp04Status
            };
            model.Errh = new WSTGErrh
            {
                Errh01Note = wstg.Errh01Note,
                Errh02Note = wstg.Errh01Note,
                Errh01Status = wstg.Errh01Status,
                Errh02Status = wstg.Errh02Status
            };
            model.Idnt = new WSTGIdnt
            {
                Idnt01Note = wstg.Idnt01Note,
                Idnt02Note = wstg.Idnt02Note,
                Idnt03Note = wstg.Idnt03Note,
                Idnt04Note = wstg.Idnt04Note,
                Idnt05Note = wstg.Idnt05Note,
                Idnt01Status = wstg.Idnt01Status,
                Idnt02Status = wstg.Idnt02Status,
                Idnt03Status = wstg.Idnt03Status,
                Idnt04Status = wstg.Idnt04Status,
                Idnt05Status = wstg.Idnt05Status
            };
            model.Inpv = new WSTGInpv
            {
                Inpv01Note = wstg.Inpv01Note,
                Inpv02Note = wstg.Inpv02Note,
                Inpv03Note = wstg.Inpv03Note,
                Inpv04Note = wstg.Inpv04Note,
                Inpv05Note = wstg.Inpv05Note,
                Inpv06Note = wstg.Inpv06Note,
                Inpv07Note = wstg.Inpv07Note,
                Inpv08Note = wstg.Inpv08Note,
                Inpv09Note = wstg.Inpv09Note,
                Inpv10Note = wstg.Inpv10Note,
                Inpv11Note = wstg.Inpv11Note,
                Inpv12Note = wstg.Inpv12Note,
                Inpv13Note = wstg.Inpv13Note,
                Inpv14Note = wstg.Inpv14Note,
                Inpv15Note = wstg.Inpv15Note,
                Inpv16Note = wstg.Inpv16Note,
                Inpv17Note = wstg.Inpv17Note,
                Inpv18Note = wstg.Inpv18Note,
                Inpv19Note = wstg.Inpv19Note,
                Inpv01Status = wstg.Inpv01Status,
                Inpv02Status = wstg.Inpv02Status,
                Inpv03Status = wstg.Inpv03Status,
                Inpv04Status = wstg.Inpv04Status,
                Inpv05Status = wstg.Inpv05Status,
                Inpv06Status = wstg.Inpv06Status,
                Inpv07Status = wstg.Inpv07Status,
                Inpv08Status = wstg.Inpv08Status,
                Inpv09Status = wstg.Inpv09Status,
                Inpv10Status = wstg.Inpv10Status,
                Inpv11Status = wstg.Inpv11Status,
                Inpv12Status = wstg.Inpv12Status,
                Inpv13Status = wstg.Inpv13Status,
                Inpv14Status = wstg.Inpv14Status,
                Inpv15Status = wstg.Inpv15Status,
                Inpv16Status = wstg.Inpv16Status,
                Inpv17Status = wstg.Inpv17Status,
                Inpv18Status = wstg.Inpv18Status,
                Inpv19Status = wstg.Inpv19Status
            };
            model.Sess = new WSTGSess
            {
                Sess01Note = wstg.Sess01Note,
                Sess02Note = wstg.Sess02Note,
                Sess03Note = wstg.Sess03Note,
                Sess04Note = wstg.Sess04Note,
                Sess05Note = wstg.Sess05Note,
                Sess06Note = wstg.Sess06Note,
                Sess07Note = wstg.Sess07Note,
                Sess08Note = wstg.Sess08Note,
                Sess09Note = wstg.Sess09Note,
                Sess01Status = wstg.Sess01Status,
                Sess02Status = wstg.Sess02Status,
                Sess03Status = wstg.Sess03Status,
                Sess04Status = wstg.Sess04Status,
                Sess05Status = wstg.Sess05Status,
                Sess06Status = wstg.Sess06Status,
                Sess07Status = wstg.Sess07Status,
                Sess08Status = wstg.Sess08Status,
                Sess09Status = wstg.Sess09Status
            };
            await base.OnInitializedAsync();

            StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.Id = checklist.Id;
            var response = await _checklistController.EditWstg(model);
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
                    MudDialog.Close(DialogResult.Ok(true));
                }
                break;
        }
        
    }
}