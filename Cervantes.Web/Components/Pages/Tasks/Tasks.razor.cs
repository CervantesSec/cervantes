using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazor.Flags;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class Tasks: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public Guid project { get; set; }

    private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
    private List<CORE.Entities.Task> tasks = new List<CORE.Entities.Task>();
    private List<CORE.Entities.Task> seleTasks = new List<CORE.Entities.Task>();
    [Parameter] public Guid id { get; set; }

    private List<ApplicationUser> users = new List<ApplicationUser>();
    private List<Project> Projects = new List<Project>();
    private string selectedUser;
    private Guid selectedProject = Guid.Empty;
    private string searchString = "";
    [Inject ]private IExportToCsv ExportToCsv { get; set; }

    [Inject] private TaskController _taskController { get; set; }
    [Inject] private UserController _userController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        await Update();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["tasks"],null ,icon: Icons.Material.Filled.Task),
        };
        
    }


    protected async Task Update()
    { 
        tasks = _taskController.Get().ToList();
        users = new List<CORE.Entities.ApplicationUser>();
        users.Clear();
        selectedUser = string.Empty;
        users = _userController.Get().ToList();
        Projects = _projectController.Get().Where(x => x.Template == false).ToList();
    }
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions mediumWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    DialogOptionsEx maxWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = true,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.CenterRight,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };

    private async Task OpenDialogCreate(Guid project,DialogOptionsEx options)
    {
        
        var parameters = new DialogParameters { ["project"]=project };
        IMudExDialogReference<CreateTaskDialog>? dlgReference = await DialogEx.ShowEx<CreateTaskDialog>("Simple Dialog", options);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }

    private async Task AssignToMe(Guid id)
    {
        var model = new TaskAssignToMeViewModel();
        model.TaskId = id;
        model.UserId = _accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _taskController.AssignToMe(model);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["taskAssigned"], Severity.Success);
        }
        else
        {
            Snackbar.Add(@localizer["taskAssignedError"], Severity.Error);
        }
        
    }

   
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Task> args)
    {
        var parameters = new DialogParameters { ["task"]=args.Item};
        IMudExDialogReference<TaskDialog>? dlgReference = await DialogEx.ShowEx<TaskDialog>("Simple Dialog",parameters, maxWidthEx);

        //var dialog =  Dialog.Show<TaskDialog>(args.Item.Name, parameters,maxWidth);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private Func<CORE.Entities.Task, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.AsignedUser.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StartDate.ToString().Contains(searchString))
            return true;
        if (element.EndDate.ToString().Contains(searchString))
            return true;
        return false;
    };
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["tasks"]=seleTasks };

                var dialog =  Dialog.Show<DeleteTaskBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Task> items)
    {
        
        seleTasks = items.ToList();
    }
    
    
    
}