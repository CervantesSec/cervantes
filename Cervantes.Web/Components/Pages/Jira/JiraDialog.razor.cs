using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Jira;

public partial class JiraDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public CORE.Entities.Jira jira { get; set; } = new CORE.Entities.Jira();

    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private JiraController _jiraController { get; set; }
    [Inject] private VulnController _vulnController { get; set; }
    private List<CORE.Entities.JiraComments> JiraComments = new List<CORE.Entities.JiraComments>();
    const string jiraSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M11.53 2c0 2.4 1.97 4.35 4.35 4.35h1.78v1.7c0 2.4 1.94 4.34 4.34 4.35V2.84a.84.84 0 0 0-.84-.84zM6.77 6.8a4.362 4.362 0 0 0 4.34 4.34h1.8v1.72a4.362 4.362 0 0 0 4.34 4.34V7.63a.841.841 0 0 0-.83-.83zM2 11.6c0 2.4 1.95 4.34 4.35 4.34h1.78v1.72c.01 2.39 1.95 4.34 4.34 4.34v-9.57a.84.84 0 0 0-.84-.84z""/></svg>";

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
  
    private JiraCommentCreate jiraComment = new JiraCommentCreate();
    public CORE.Entities.Vuln vuln { get; set; } = new CORE.Entities.Vuln();
    private ClaimsPrincipal userAth;

    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        vuln = _vulnController.GetById(jira.VulnId);
        JiraComments = _jiraController.GetCommentsByVuln(vuln.Id).ToList();
        await base.OnInitializedAsync();
        StateHasChanged();
    }
    
    private async Task CreateJira()
    {
        var response = await _jiraController.Add(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraCreated"], Severity.Success);
            await UpdateJira();
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["jiraCreatedError"], Severity.Error);
        }
    }
    
    private async Task DeleteJira()
    {
        
        var response = await _jiraController.DeleteIssue(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraDeleted"], Severity.Success);
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["jiraDeletedError"], Severity.Error);
        }
    }
    
    private async Task UpdateJira()
    {
        var response = await _jiraController.UpdateIssue(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraUpdated"], Severity.Success);
            jira = _jiraController.GetJiraByVuln(vuln.Id);
            JiraComments = _jiraController.GetCommentsByVuln(vuln.Id).ToList();
        }
        else
        {
            Snackbar.Add(@localizer["jiraUpdatedError"], Severity.Error);
        }
    }

    private string test;
    private async Task AddComment()
    {
        jiraComment = new JiraCommentCreate
        {
            VulnId = vuln.Id,
            Comment = test
        };
        var response = await _jiraController.AddComment(jiraComment);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["addedComment"], Severity.Success);
            await UpdateJira();

        }
        else
        {
            Snackbar.Add(@localizer["addedCommentError"], Severity.Error);
        }
    }

    
}