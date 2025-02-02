using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class UserDialog: ComponentBase
{
    [Parameter] public UserViewModel userSelected { get; set; } 
    private ApplicationUser user = new ApplicationUser();
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private UserController _UserController { get; set; }
    [Inject] private ClientsController _ClientsController { get; set; }
    [Inject] private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager { get; set; } 

    MudForm form;
    private List<RolesViewModel> Roles = new List<RolesViewModel>();
    private List<CORE.Entities.Client> Clients = new List<CORE.Entities.Client>();
    private static IBrowserFile File;
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
    private UserEditViewModel model { get; set; } = new UserEditViewModel();
    ClaimsPrincipal userAth;
    private string roleUsr;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        user = _UserController.GetUser(userSelected.Id);
        roleUsr = await _UserController.GetRole(user.Id);
    }
    
    async Task DeleteDialog(DialogOptions options)
    {
        var parameters = new DialogParameters { ["user"]=userSelected };

        var dialog =  Dialog.Show<DeleteUserDialog>("Edit", parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            MudDialog.Close();
        }
    }

    async Task EditMode()
    {
        if (editMode)
        {
            editMode = false;
        }
        else
        {
            editMode = true;
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
            model.Id = user.Id;
            model.Email = user.Email;
            model.FullName = user.FullName;
            model.Description = user.Description;
            model.PhoneNumber = user.PhoneNumber;
            model.Position = user.Position;
            model.ImagePath = user.Avatar;
            model.TwoFactorEnabled = user.TwoFactorEnabled;
            model.ExternalLogin = user.ExternalLogin;
            bool locked = false;
            if (user.LockoutEnd != null)
            {
                locked = true;
            }
            else
            {
                locked = false;
            }
            model.Lockout = locked;
            if (user.ClientId != null)
            {
                model.ClientId = user.ClientId;
            }
            //var user2 =  _UserController.GetUser(user.Id);
            //var rolUser = await _userManager.GetRolesAsync(user2);
            model.Role = roleUsr;
        }
        MudDialog.StateHasChanged();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (File != null)
            {
                Stream stream = File.OpenReadStream();
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();
	        
                model.FileName = File.Name;
                model.FileContent = ms.ToArray();
                ms.Close();
                File = null;
            }

            var response = await _UserController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(localizer["userEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(localizer["userEditedError"], Severity.Error);
                StateHasChanged();
            }
            
        }
    }
    UserModelFluentValidator userModelFluentValidator = new UserModelFluentValidator();

    public class UserModelFluentValidator : AbstractValidator<UserEditViewModel>
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
            RuleFor(p => p.Password)
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x => x.ConfirmPassword)
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.Role).NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserEditViewModel>.CreateWithOptions((UserEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task DeleteAvatar()
    {
            var response = await _UserController.DeleteAvatar(model.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(localizer["userEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(localizer["userEditedError"], Severity.Error);
                StateHasChanged();
            }
        
    }
    
}