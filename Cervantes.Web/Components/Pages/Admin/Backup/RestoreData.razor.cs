using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Backup;
   
public partial class RestoreData: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    BackupFormViewModel model = new BackupFormViewModel();
    [Inject] private BackupController backupController { get; set; }
    	private long maxFileSize = 1024 * 1024 * 500;
    private IBrowserFile file;
        
    private async Task Submit()
    {
        try
        {
            if (file != null)
            {
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();

                model.FileName = file.Name;
                model.FileContent = ms.ToArray();
                ms.Close();
                file = null;
                
                var response = await backupController.RestoreData(model);
                
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["restoredData"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(@localizer["restoreDataError"], Severity.Error);
                }
            }
            else
            {
                Snackbar.Add(@localizer["restoreError"], Severity.Error);
            }


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }

}