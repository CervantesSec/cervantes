using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Jira;
using Cervantes.Web.Components.Pages.Tasks;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Workspace.Vault;

public partial class VaultDialog: ComponentBase
{
    [Inject] private VaultController _VaultController { get; set; }
    [Parameter] public CORE.Entities.Vault vault { get; set; } 

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private VaultEditViewModel model = new VaultEditViewModel();
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool editMode = false;

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
            ClaimsPrincipal userAth;
            DialogOptionsEx centerWidthEx = new DialogOptionsEx() 
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
                Position = DialogPosition.Center,
                DisableSizeMarginY = true,
                DisablePositionMargin = true,
                BackdropClick = false,
                Resizeable = true,
            };
            DialogOptionsEx middleWidthEx = new DialogOptionsEx() 
            {
                MaximizeButton = true,
                CloseButton = true,
                FullHeight = false,
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                MaxHeight = MaxHeight.False,
                FullWidth = true,
                DragMode = MudDialogDragMode.Simple,
                Animations = new[] { AnimationType.SlideIn },
                Position = DialogPosition.Center,
                DisableSizeMarginY = true,
                DisablePositionMargin = true,
                BackdropClick = false,
                Resizeable = true,
            };
            protected override async Task OnInitializedAsync()
            {
                await base.OnInitializedAsync();
                userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            }
    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var response = await _VaultController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {

                Snackbar.Add(@localizer["vaultEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["vaultEditedError"], Severity.Error);
            }

        }
    }
    
    void EditMode()
    {
        if (editMode)
        {
            editMode = false;
        }
        else
        {
            editMode = true;
            model.Id = vault.Id;
            model.Name = vault.Name;
            model.Description = vault.Description;
            model.ProjectId = vault.ProjectId;
            model.Type = vault.Type;
            model.Value = vault.Value;

        }
        MudDialog.StateHasChanged();
    }
    VaultModelFluentValidator vaultValidator = new VaultModelFluentValidator();

    public class VaultModelFluentValidator : AbstractValidator<VaultEditViewModel>
    {
        public VaultModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Value)
                .NotEmpty();
      
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VaultEditViewModel>.CreateWithOptions((VaultEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task DeleteVaultDialog(CORE.Entities.Vault item,DialogOptions options)
    {
        var parameters = new DialogParameters { ["vault"]=item };
        IMudExDialogReference<DeleteVaultDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVaultDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
            StateHasChanged();
        }
    }
    
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    bool isShow;
    InputType PasswordInput = InputType.Password;

    void ValueClick()
    {
        if(isShow)
        {
            isShow = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else {
            isShow = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
}