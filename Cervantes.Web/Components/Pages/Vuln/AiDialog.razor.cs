using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class AiDialog: ComponentBase
{
    private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] IAiService aiService { get; set; }
    MudForm form;

    private DefaultModel defaultModel = new DefaultModel();
    private CustomModel customModel = new CustomModel();

    bool _isBusy = false;
    private string result;
    private string result2;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync(); 
    }

    public class DefaultModel
    {
        public string Name { get; set; }
        public Language Language { get; set; }
    }
    DefaultFluentValidator defaultValidator = new DefaultFluentValidator();

    public class DefaultFluentValidator : AbstractValidator<DefaultModel>
    {
        public DefaultFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DefaultModel>.CreateWithOptions((DefaultModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task SubmitDefault() 
    {
        await form.Validate();

        if (form.IsValid)
        {
            _isBusy = true;
            var result = await aiService.GenerateVuln(defaultModel.Name, defaultModel.Language);
            MudDialog.Close<VulnAiModel>(result);  
        }
        
    } 
    
    public class CustomModel
    {
        public string Prompt { get; set; }
    }
    
    
    CustomFluentValidator customValidator = new CustomFluentValidator();

    public class CustomFluentValidator : AbstractValidator<CustomModel>
    {
        public CustomFluentValidator()
        {
            RuleFor(x => x.Prompt)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CustomModel>.CreateWithOptions((CustomModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task SubmitCustom() 
    {
        await form.Validate();

        if (form.IsValid)
        {
            _isBusy = true;
            var gen = await aiService.GenerateCustom(customModel.Prompt);
            result2 = gen;
            _isBusy = false;

        }
        
    } 
}