using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class TasksKanban : ComponentBase
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
            new BreadcrumbItem(@localizer["tasks"], href: "/",icon: Icons.Material.Filled.Task),

            new BreadcrumbItem("Kanban",null ,icon: Icons.Material.Filled.ViewKanban),
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
    
    private async Task ItemUpdated(MudItemDropInfo<CORE.Entities.Task> dropItem)
    {
        var status = dropItem.Item.Status;
        dropItem.Item.Status = (CORE.Entities.TaskStatus)Enum.Parse(typeof(CORE.Entities.TaskStatus), dropItem.DropzoneIdentifier);
        var update = new TaskUpdateViewModel();
        update.Id = dropItem.Item.Id;
        update.Status = dropItem.Item.Status;
        var response = await _taskController.UpdateStatus(update);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["taskUpdated"], Severity.Success);
        }
        else if (response is BadRequestObjectResult badRequestResult)
        {
            dropItem.Item.Status = status;
            var message = badRequestResult.Value;
            if (message.ToString() == "NotAllowed")
            {
                Snackbar.Add(@localizer["noInProject"], Severity.Warning);
            }
			        
        }
        else
        {
            dropItem.Item.Status = status;

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


    
    async Task DetailsDialog(CORE.Entities.Task task, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["task"]=task};

        IMudExDialogReference<TaskDialog>? dlgReference = await DialogEx.ShowEx<TaskDialog>("Simple Dialog",parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
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
    
    
    
}