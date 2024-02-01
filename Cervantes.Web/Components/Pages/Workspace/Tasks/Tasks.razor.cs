using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Tasks;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Tasks;

public partial class Tasks: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public Guid project { get; set; }
    private CORE.Entities.Project Project = new CORE.Entities.Project();
 private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
    private List<CORE.Entities.Task> tasks = new List<CORE.Entities.Task>();
    private List<CORE.Entities.Task> seleTasks = new List<CORE.Entities.Task>();
    private List<ApplicationUser> users = new List<ApplicationUser>();
    private List<CORE.Entities.Project> Projects = new List<CORE.Entities.Project>();
    private string selectedUser;
    private Guid selectedProject = Guid.Empty;
    private string searchString = "";

    [Inject] private TaskController _taskController { get; set; }
    [Inject] private UserController _userController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    protected override async Task OnInitializedAsync()
    {
        var user = await _projectController.VerifyUser(project);
        if (user == false)
        {
            Snackbar.Add(@localizer["noRolePermission"], Severity.Warning);
            navigationManager.NavigateTo("workspaces");
        }
        await Update();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(Project.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["tasks"], href: null, disabled: true, icon: Icons.Material.Filled.Task)
        };
        await base.OnInitializedAsync();
    }
    
    protected async Task Update()
    { 
        tasks = _taskController.GetByProject(project).ToList();
        users = new List<CORE.Entities.ApplicationUser>();
        users.Clear();
        selectedUser = string.Empty;
        users = _userController.Get().ToList();
        Project = _projectController.GetById(project);
    }
    
    private async Task ItemUpdated(MudItemDropInfo<CORE.Entities.Task> dropItem)
    {
        dropItem.Item.Status = (CORE.Entities.TaskStatus)Enum.Parse(typeof(CORE.Entities.TaskStatus), dropItem.DropzoneIdentifier);
        var update = new TaskUpdateViewModel();
        update.Id = dropItem.Item.Id;
        update.Status = dropItem.Item.Status;
        var response = await _taskController.UpdateStatus(update);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["taskUpdated"], Severity.Success);
        }
        else
        {
            Snackbar.Add(@localizer["taskUpdatedError"], Severity.Error);
        }
    }
    
    private void Filter()
    {
        if (selectedUser != string.Empty)
        {
           tasks = tasks.Where(x => x.AsignedUserId == selectedUser).ToList();
        }
        if (selectedProject != Guid.Empty)
        {
            tasks = tasks.Where(x => x.ProjectId == selectedProject).ToList();
        }
        StateHasChanged();
    }
    
    private void ClearFilter()
    {
        tasks = _taskController.Get().ToList();
            selectedProject = Guid.Empty;
            selectedUser = string.Empty;
            StateHasChanged();
    }
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    private async Task OpenDialogCreate(Guid project,DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=Project.Id };
        var dialog = Dialog.Show<CreateTaskDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }

    
    async Task DeleteDialog(CORE.Entities.Task task, Guid project, DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task, ["project"]=project };

        var dialog =  Dialog.Show<DeleteTaskDialog>(@localizer["delete"], parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            tasks.Remove(task);
            StateHasChanged();
        }
    }
    
    async Task DetailsDialog(CORE.Entities.Task task, DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task};

        var dialog =  Dialog.Show<TaskDialog>(task.Name, parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            StateHasChanged();
        }
    }
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Task> args)
    {
        var parameters = new DialogParameters { ["task"]=args.Item};

        var dialog =  Dialog.Show<TaskDialog>(args.Item.Name, parameters,maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
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
        if (element.Project.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StartDate.ToString().Contains(searchString))
            return true;
        if (element.EndDate.ToString().Contains(searchString))
            return true;
        return false;
    };
    
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var records = model.Select(e => new IFR.Export.ProjectExport()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Client = e.Client.Name,
                    CreatedUser = e.User.FullName,
                    StartDate = e.StartDate.ToShortDateString(),
                    EndDate = e.EndDate.ToShortDateString(),
                    Template = e.Template,
                    Status = e.Status.ToString(),
                    ProjectType = e.ProjectType.ToString(),
                    Language = e.Language.ToString(),
                    Score = e.Score.ToString(),
                    FindingsId = e.FindingsId.ToString(),
                    ExecutiveSummary = e.ExecutiveSummary
                    
 
                }).ToList();
                var file = ExportToCsv.ExportProjects(records);
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
                var parameters = new DialogParameters { ["tasks"]=seleTasks };

                var dialog =  Dialog.Show<DeleteTaskBulkDialog>("Edit", parameters,maxWidth);
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