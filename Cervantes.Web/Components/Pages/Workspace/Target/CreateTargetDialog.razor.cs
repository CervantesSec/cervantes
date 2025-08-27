using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Cervantes.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Microsoft.EntityFrameworkCore;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class CreateTargetDialog: ComponentBase
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
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] TargetController _targetController { get; set; }
    [Inject] TargetCustomFieldController _targetCustomFieldController { get; set; }
    TargetCreateViewModel target = new TargetCreateViewModel();
    [Parameter] public Guid project { get; set; }
    
    // Custom fields
    private List<TargetCustomFieldValueViewModel> CustomFields { get; set; } = new List<TargetCustomFieldValueViewModel>();

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadCustomFields();
        StateHasChanged();
    }
    
    private async System.Threading.Tasks.Task LoadCustomFields()
    {
        try
        {
            var customFields = _targetCustomFieldController.GetActive();
            CustomFields = customFields.Select(cf => new TargetCustomFieldValueViewModel
            {
                TargetCustomFieldId = cf.Id,
                Name = cf.Name,
                Label = cf.Label,
                Type = cf.Type,
                IsRequired = cf.IsRequired,
                IsUnique = cf.IsUnique,
                IsSearchable = cf.IsSearchable,
                IsVisible = cf.IsVisible,
                Order = cf.Order,
                Options = cf.Options,
                DefaultValue = cf.DefaultValue,
                Description = cf.Description,
                Value = cf.DefaultValue ?? string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", Severity.Error);
        }
    }
    
    private async System.Threading.Tasks.Task OnCustomFieldChanged(TargetCustomFieldValueViewModel field)
    {
        // Initialize CustomFieldValues dictionary if null
        if (target.CustomFieldValues == null)
        {
            target.CustomFieldValues = new Dictionary<Guid, string>();
        }
        
        // Update the target's custom field values
        target.CustomFieldValues[field.TargetCustomFieldId] = field.Value;
        await InvokeAsync(StateHasChanged);
    }
    
    TargetModelFluentValidator targetValidator = new TargetModelFluentValidator();
    public class TargetModelFluentValidator : AbstractValidator<TargetCreateViewModel>
    {
        public TargetModelFluentValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty();
        }
	    

        public Func<object, string, System.Threading.Tasks.Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TargetCreateViewModel>.CreateWithOptions((TargetCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            target.ProjectId = project;

            var response = await _targetController.Add(target);
            if (response is Microsoft.AspNetCore.Mvc.CreatedResult)
            {
                Snackbar.Add(@localizer["targetCreated"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["targetCreatedError"], Severity.Error);
            }
        }
    }
}