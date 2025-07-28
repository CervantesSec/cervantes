using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Cervantes.Web.Localization;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Components.Pages.Cve;

public partial class CveCreateDialog
{
    [CascadingParameter] IDialogReference? MudDialog { get; set; } = default!;



    private MudForm form;
    private CveCreateViewModel model = new();
    private bool loading = false;
    private DateTime? lastModifiedDate
    {
        get => model.LastModifiedDate;
        set => model.LastModifiedDate = value;
    }

    private DateTime? publishedDate
    {
        get => model.PublishedDate == default(DateTime) ? null : model.PublishedDate;
        set => model.PublishedDate = value ?? DateTime.UtcNow;
    }

    protected override async Task OnInitializedAsync()
    {
        model.SetDefaults();
    }

    private async Task CreateCve()
    {
        loading = true;
        try
        {
            await form.Validate();
            if (!form.IsValid)
            {
                return;
            }

            var validationErrors = model.Validate();
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    Snackbar.Add(error, Severity.Error);
                }
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cve = new CORE.Entities.Cve
            {
                Id = Guid.NewGuid(),
                CveId = model.CveId,
                Title = model.Title,
                Description = model.Description,
                PublishedDate = model.PublishedDate,
                LastModifiedDate = model.LastModifiedDate ?? model.PublishedDate,
                CvssV3BaseScore = model.CvssV3BaseScore,
                CvssV3Vector = model.CvssV3Vector,
                CvssV3Severity = model.CvssV3Severity,
                CvssV2BaseScore = model.CvssV2BaseScore,
                CvssV2Vector = model.CvssV2Vector,
                CvssV2Severity = model.CvssV2Severity,
                EpssScore = model.EpssScore,
                EpssPercentile = model.EpssPercentile,
                IsKnownExploited = model.IsKnownExploited,
                KevDueDate = model.KevDueDate,
                PrimaryCweId = model.PrimaryCweId,
                PrimaryCweName = model.PrimaryCweName,
                State = model.State,
                AssignerOrgId = model.AssignerOrgId,
                SourceIdentifier = model.SourceIdentifier,
                Notes = model.Notes,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                UserId = userId
            };

            await CveManager.AddAsync(cve);
            MudDialog.Close(DialogResult.Ok(cve));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error creating CVE: {ex.Message}", Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close();
    }

    private Func<object, string, Task<IEnumerable<string>>> ValidateModel => async (model, propertyName) =>
    {
        var cveModel = (CveCreateViewModel)model;
        var errors = new List<string>();

        if (propertyName == nameof(CveCreateViewModel.CveId))
        {
            if (string.IsNullOrEmpty(cveModel.CveId))
            {
                errors.Add("CVE ID is required");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(cveModel.CveId, @"^CVE-\d{4}-\d{4,}$"))
            {
                errors.Add("Invalid CVE ID format. Expected format: CVE-YYYY-NNNN");
            }
            else
            {
                // Check if CVE already exists
                var existingCve = await CveManager.GetByCveIdAsync(cveModel.CveId);
                if (existingCve != null)
                {
                    errors.Add("CVE ID already exists");
                }
            }
        }

        if (propertyName == nameof(CveCreateViewModel.Title) && string.IsNullOrEmpty(cveModel.Title))
        {
            errors.Add("Title is required");
        }

        if (propertyName == nameof(CveCreateViewModel.Description) && string.IsNullOrEmpty(cveModel.Description))
        {
            errors.Add("Description is required");
        }

        if (propertyName == nameof(CveCreateViewModel.CvssV3BaseScore) && cveModel.CvssV3BaseScore.HasValue)
        {
            if (cveModel.CvssV3BaseScore < 0 || cveModel.CvssV3BaseScore > 10)
            {
                errors.Add("CVSS v3 base score must be between 0.0 and 10.0");
            }
        }

        if (propertyName == nameof(CveCreateViewModel.EpssScore) && cveModel.EpssScore.HasValue)
        {
            if (cveModel.EpssScore < 0 || cveModel.EpssScore > 1)
            {
                errors.Add("EPSS score must be between 0.0 and 1.0");
            }
        }

        if (propertyName == nameof(CveCreateViewModel.KevDueDate))
        {
            if (cveModel.IsKnownExploited && !cveModel.KevDueDate.HasValue)
            {
                errors.Add("KEV due date is required for known exploited CVEs");
            }
        }

        return errors;
    };
}