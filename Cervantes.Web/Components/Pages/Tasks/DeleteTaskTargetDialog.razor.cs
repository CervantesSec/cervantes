using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class DeleteTaskTargetDialog: ComponentBase
{
	[CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
	void Cancel() => MudDialog.Cancel();
     
	[Inject] ISnackbar Snackbar { get; set; }
	[Inject] TaskController _taskController { get; set; }
	MudForm form;


	[Parameter] public TaskTargets target { get; set; }
	 

	private async System.Threading.Tasks.Task Submit()
	{
		await form.Validate();

		if (form.IsValid)
		{

			var response = await _taskController.DeleteTarget(target.Id);
			if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
			{
				Snackbar.Add(@localizer["targetDeleted"], Severity.Success);
				MudDialog.Close(DialogResult.Ok(true));
			}
			else
			{
				Snackbar.Add(@localizer["targetDeletedError"], Severity.Error);
			}
            
		}
	}
}