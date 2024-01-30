using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Blazor.Flags;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
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
    protected override async Task OnInitializedAsync()
    {
        
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
        Projects = _projectController.Get().ToList();
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
        var parameters = new DialogParameters { ["project"]=project };
        var dialog = Dialog.Show<CreateTaskDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
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
        try
        {
            switch (id)
            {
                case 0:
                    var records = tasks.Select(e => new IFR.Export.TaskExport()
                    {
                        Name = e.Name,
                        Description = e.Description,
                        Project = e.Project?.Name,
                        AsignedUser = e.AsignedUser?.FullName,
                        StartDate = e.StartDate.ToShortDateString(),
                        EndDate = e.EndDate.ToShortDateString(),
                        Template = e.Template,
                        Status = e.Status.ToString(),
                    }).ToList();
                    foreach (var record in records)
                    {
                        if (string.IsNullOrEmpty(record.Project))
                        {
                            record.Project = "No Project";
                        }
                    }
                    var file = ExportToCsv.ExportTasks(records);
                    await JS.InvokeVoidAsync("downloadFile", file);
                    Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                    ExportToCsv.DeleteFile(file);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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