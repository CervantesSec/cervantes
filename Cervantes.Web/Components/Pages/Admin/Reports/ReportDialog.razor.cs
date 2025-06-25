using System.Net;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Components.Shared;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class ReportDialog : ComponentBase
{
    [Parameter] public CORE.Entities.ReportTemplate report { get; set; } = new CORE.Entities.ReportTemplate();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ReportController _reportController { get; set; }
    MudForm form;
    private static IBrowserFile file;
    [Parameter] public EditReportTemplateModel model { get; set; } = new EditReportTemplateModel();
    ReportTemplateModelFluentValidator templateValidator = new ReportTemplateModelFluentValidator();


    private Dictionary<string, object> editorConf = new Dictionary<string, object>
    {
        {
            "plugins",
            "preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons"
        },
        { "menubar", "file edit view insert format tools table help" },
        {
            "toolbar",
            "undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media link anchor codesample | ltr rtl"
        },
        { "toolbar_sticky", true },
        { "image_advtab", true },
        { "height", 300 },
        { "image_caption", true },
        { "promotion", false },
        { "quickbars_selection_toolbar", "bold italic | quicklink h2 h3 blockquote quickimage quicktable" },
        { "noneditable_noneditable_class", "mceNonEditable" },
        { "toolbar_mode", "sliding" },
        { "contextmenu", "link image imagetools table" },
        {
            "textpattern_patterns", new object[]
            {
                new { start = "#", format = "h1" },
                new { start = "##", format = "h2" },
                new { start = "###", format = "h3" },
                new { start = "####", format = "h4" },
                new { start = "#####", format = "h5" },
                new { start = "######", format = "h6" },
                new { start = ">", format = "blockquote" },
                new { start = "*", end = "*", format = "italic" },
                new { start = "_", end = "_", format = "italic" },
                new { start = "**", end = "**", format = "bold" },
                new { start = "__", end = "__", format = "bold" },
                new { start = "***", end = "***", format = "bold italic" },
                new { start = "___", end = "___", format = "bold italic" },
                new { start = "__*", end = "*__", format = "bold italic" },
                new { start = "**_", end = "_**", format = "bold italic" },
                new { start = "`", end = "`", format = "code" },
                new { start = "---", replacement = "<hr/>" },
                new { start = "--", replacement = "—" },
                new { start = "-", replacement = "—" },
                new { start = "(c)", replacement = "©" },
                new { start = "~", end = "~", cmd = "createLink" },
                new { start = "<", end = ">", cmd = "createLink" },
                new { start = "* ", cmd = "InsertUnorderedList" },
                new { start = "-", cmd = "InsertUnorderedList" },
                new { start = "1. ", cmd = "InsertOrderedList", value = "decimal" },
                new { start = "1) ", cmd = "InsertOrderedList", value = "decimal" },
                new { start = "a. ", cmd = "InsertOrderedList", value = "lower-alpha" },
                new { start = "a) ", cmd = "InsertOrderedList", value = "lower-alpha" },
                new { start = "i. ", cmd = "InsertOrderedList", value = "lower-roman" },
                new { start = "i) ", cmd = "InsertOrderedList", value = "lower-roman" }
            }
        }
    };

    private ClaimsPrincipal userAth;
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
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var comp = _reportController.Components().ToList();
        var parts = _reportController.GetParts(report.Id).ToList();
        _dropItems = new List<DropItem>();
        foreach (var com in comp)
        {
            var item = new DropItem();
            if (parts.Any(x => x.ComponentId == com.Id))
            {
                item.Id = com.Id;
                item.Name = com.Name;
                item.Language = com.Language;
                item.ComponentType = com.ComponentType;
                item.Identifier = com.ComponentType.ToString();
                item.Order = parts.FirstOrDefault(x => x.ComponentId == com.Id).Order;
            }
            else
            {
                item.Id = com.Id;
                item.Name = com.Name;
                item.Language = com.Language;
                item.ComponentType = com.ComponentType;
                item.Identifier = "None";
            }

            _dropItems.Add(item);
        }

        await base.OnInitializedAsync();
    }


    async Task DeleteDialog(CORE.Entities.ReportTemplate report, DialogOptions options)
    {
        var parameters = new DialogParameters { ["report"] = report };
        IMudExDialogReference<DeleteReportDialog>? dlgReference = await Dialog.ShowExAsync<DeleteReportDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close();
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
            model.Id = report.Id;
            model.Name = report.Name;
            model.Description = report.Description;
            model.Language = report.Language;
        }

        MudDialog.StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var itemsTemplate = _dropItems.Where(x => x.Identifier != "None" && x.Language == model.Language).ToList();
            model.Components = new List<ReportPartsModel>();
            foreach (var item in itemsTemplate)
            {
                var part = new ReportPartsModel();
                part.Id = item.Id;
                part.Order = item.Order;
                model.Components.Add(part);
            }

            var response = await _reportController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["reportTemplateEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["reportTemplateEditedError"], Severity.Error);
            }
        }
    }

    public class ReportTemplateModelFluentValidator : AbstractValidator<EditReportTemplateModel>
    {
        public ReportTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 100);
        }


        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result =
                await ValidateAsync(
                    ValidationContext<EditReportTemplateModel>.CreateWithOptions((EditReportTemplateModel)model,
                        x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    /*public async Task DownloadTemplate()
    {
        await JS.InvokeVoidAsync("downloadFile", report.FilePath);
        Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
    }*/

    private void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
    {
        dropItem.Item.Identifier = dropItem.DropzoneIdentifier;
        dropItem.Item.Order = _dropItems.Count(item => item.Identifier == dropItem.DropzoneIdentifier);
    }

    private List<DropItem> _dropItems = new();


    public class DropItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Language Language { get; set; }
        public ReportPartType ComponentType { get; set; }

        public string Identifier { get; set; }
        public int Order { get; set; }
    }
}