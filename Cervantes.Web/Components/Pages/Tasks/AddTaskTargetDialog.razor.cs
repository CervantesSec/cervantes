using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class AddTaskTargetDialog : ComponentBase
{

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    List<CORE.Entities.Target> targets { get; set; } = new List<CORE.Entities.Target>();
    TaskTargetViewModel model = new TaskTargetViewModel();
    [Parameter] public CORE.Entities.Task task { get; set; }
    
    [Inject] private TargetController _targetController { get; set; }
    [Inject] private TaskController _taskController { get; set; }

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        targets = new List<CORE.Entities.Target>();
        targets = _targetController.GetByProjectId(task.ProjectId.Value).ToList();
        StateHasChanged();
    }
    

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            model.TaskId = task.Id;

            var response = await _taskController.AddTarget(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedResult")
            {
                    Snackbar.Add(@localizer["targetAdded"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
            }
            else if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkObjectResult")
            {
                Snackbar.Add(@localizer["targetExists"], Severity.Info);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["targetAddedError"], Severity.Error);
            }

        }
    }
}