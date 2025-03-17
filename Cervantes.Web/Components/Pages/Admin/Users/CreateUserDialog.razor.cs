using System.Net;
using Microsoft.AspNetCore.Components;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Variant = MudBlazor.Variant;
using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class CreateUserDialog: ComponentBase
{
private Dictionary<string, object> editorConf = new Dictionary<string, object>{
                {"plugins", "preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons"},
                {"menubar", "file edit view insert format tools table help"},
                {"toolbar", "undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media link anchor codesample | ltr rtl"},
                {"toolbar_sticky", true},
                {"image_advtab", true},
                {"height", 300},
                {"image_caption", true},
                {"promotion", false},
                {"quickbars_selection_toolbar", "bold italic | quicklink h2 h3 blockquote quickimage quicktable"},
                {"noneditable_noneditable_class", "mceNonEditable"},
                {"toolbar_mode", "sliding"},
                {"contextmenu", "link image imagetools table"},
                {"textpattern_patterns", new object[] {
                    new {start = "#", format = "h1"},
                    new {start = "##", format = "h2"},
                    new {start = "###", format = "h3"},
                    new {start = "####", format = "h4"},
                    new {start = "#####", format = "h5"},
                    new {start = "######", format = "h6"},
                    new {start = ">", format = "blockquote"},
                    new {start = "*", end = "*", format = "italic"},
                    new {start = "_", end = "_", format = "italic"},
                    new {start = "**", end = "**", format = "bold"},
                    new {start = "__", end = "__", format = "bold"},
                    new {start = "***", end = "***", format = "bold italic"},
                    new {start = "___", end = "___", format = "bold italic"},
                    new {start = "__*", end = "*__", format = "bold italic"},
                    new {start = "**_", end = "_**", format = "bold italic"},
                    new {start = "`", end = "`", format = "code"},
                    new {start = "---", replacement = "<hr/>"},
                    new {start = "--", replacement = "—"},
                    new {start = "-", replacement = "—"},
                    new {start = "(c)", replacement = "©"},
                    new {start = "~", end = "~", cmd = "createLink"},
                    new {start = "<", end = ">", cmd = "createLink"},
                    new {start = "* ", cmd = "InsertUnorderedList"},
                    new {start = "-", cmd = "InsertUnorderedList"},
                    new {start = "1. ", cmd = "InsertOrderedList", value = "decimal"},
                    new {start = "1) ", cmd = "InsertOrderedList", value = "decimal"},
                    new {start = "a. ", cmd = "InsertOrderedList", value = "lower-alpha"},
                    new {start = "a) ", cmd = "InsertOrderedList", value = "lower-alpha"},
                    new {start = "i. ", cmd = "InsertOrderedList", value = "lower-roman"},
                    new {start = "i) ", cmd = "InsertOrderedList", value = "lower-roman"}
                }}
            };
	
         [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private UserController _UserController { get; set; }
     [Inject] private ClientsController _ClientsController { get; set; }

    MudForm form;

    UserModelFluentValidator clientValidator = new UserModelFluentValidator();
    private long maxFileSize = 1024 * 1024 * 5;

    UserCreateViewModel model = new UserCreateViewModel();
    private List<RolesViewModel> Roles = new List<RolesViewModel>();
    private List<CORE.Entities.Client> Clients = new List<CORE.Entities.Client>();
    private static IBrowserFile File;
    private string response2;
    private Guid clientId = Guid.Empty;
	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
	        if (File != null)
	        {
		        Stream stream = File.OpenReadStream(maxFileSize);
		        MemoryStream ms = new MemoryStream();
		        await stream.CopyToAsync(ms);
		        stream.Close();
	        
		        model.FileName = File.Name;
		        model.FileContent = ms.ToArray();
		        ms.Close();
		        File = null;
	        }

	        if (clientId != Guid.Empty)
	        {
		        model.ClientId = clientId;
	        }
	        else
	        {
		        model.ClientId = null;
	        }	
	        
	        var response = await _UserController.Add(model);
	        response2 = response.ToString();
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
	        {
		        Snackbar.Add("User created successfully!", Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add("An error occurred creating the User!", Severity.Error);
	        }
            
        }
    }
	 
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Roles.RemoveAll(item => true);
		var roles = _UserController.GetRoles().ToList();
		foreach (var item in roles)
		{
			Roles.Add(new RolesViewModel
			{
				Name = item.RoleName,
				Description = item.Description,
				PermmissioNumber = item.PermissionNames.Count
                
			});
		}
		Clients = new List<CORE.Entities.Client>();
		Clients = _ClientsController.Get().ToList();
		StateHasChanged();
	}
    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ClientModel"></typeparam>
    public class UserModelFluentValidator : AbstractValidator<UserCreateViewModel>
    {
        public UserModelFluentValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Email)
	            .Cascade(CascadeMode.Stop)
	            .NotEmpty()
	            .EmailAddress();
            When(x => !x.ExternalLogin, () =>
            {
	            RuleFor(p => p.Password).NotEmpty()
		            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
		            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
		            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
		            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
		            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");

	            RuleFor(x => x.ConfirmPassword).NotEmpty()
		            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
		            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
		            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
		            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
		            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");

	            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
            });
	        RuleFor(x => x.Role)
		        .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserCreateViewModel>.CreateWithOptions((UserCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
	
}