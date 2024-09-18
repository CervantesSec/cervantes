using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class ImportReport : ComponentBase
{
    private List<BreadcrumbItem> _items;
    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["reports"], href: "/",icon: Icons.Custom.FileFormats.FilePdf),
            new BreadcrumbItem(@localizer["import"], href: null, disabled: true,icon: Icons.Material.Filled.UploadFile)
        };
    }
    
     private static IBrowserFile file;

     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private ReportController _ReportController { get; set; }


    MudForm form;

    ImportModelFluentValidator importValidator = new ImportModelFluentValidator();
	 
    ReportImportViewModel model = new ReportImportViewModel();

    
	private long maxFileSize = 1024 * 1024 * 50;
	
	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
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
	        }
	        
	        var response =  await _ReportController.Import(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
	        {
		        Snackbar.Add(@localizer["clientCreated"], Severity.Success);
	        }
	        else
	        {
		        Snackbar.Add(@localizer["clientCreatedError"], Severity.Error);
	        }

        }
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ClientModel"></typeparam>
    public class ImportModelFluentValidator : AbstractValidator<ReportImportViewModel>
    {
        public ImportModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
          
		}
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ReportImportViewModel>.CreateWithOptions((ReportImportViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}