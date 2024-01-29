using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class AddMembersProjectDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    List<ApplicationUser> users { get; set; } = new List<ApplicationUser>();
    MemberViewModel member = new MemberViewModel();
    [Parameter] public Guid project { get; set; }
    [Inject] private UserController _userController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        users =  _userController.Get().ToList();
        StateHasChanged();
    }
    
    MemberModelFluentValidator memberValidator = new MemberModelFluentValidator();
    public class MemberModelFluentValidator : AbstractValidator<MemberViewModel>
    {
        public MemberModelFluentValidator()
        {

            RuleFor(x => x.MemberId)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<MemberViewModel>.CreateWithOptions((MemberViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            member.ProjectId = project;


            var response = await _projectController.AddMember(member);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")            
            {
                    Snackbar.Add(@localizer["userAdded"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                
            }
            else if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkObjectResult")
            {
                Snackbar.Add(@localizer["userExists"], Severity.Info);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["userAddedError"], Severity.Error);
            }

        }
    }
}