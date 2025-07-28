using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Cervantes.Web.Controllers;
using Cervantes.CORE;
using global::AuthPermissions.BaseCode.PermissionsCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Components.Pages.Cve;

[Authorize]
public partial class CveSubscriptions
{
    
    private List<BreadcrumbItem> breadcrumbs = new();

    private List<CveSubscription> subscriptions = new();
    private CveUserSubscriptionSummary userSummary;
    private bool loading = true;
    const string cveSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M24 21.172l-5.66-5.659c1.047-1.581 1.66-3.475 1.66-5.513 0-5.523-4.477-10-10-10s-10 4.477-10 10 4.477 10 10 10c2.038 0 3.932-.613 5.512-1.66l5.66 5.66 2.828-2.828zm-22-11.172c0-4.411 3.589-8 8-8s8 3.589 8 8-3.589 8-8 8-8-3.589-8-8zm13 1.006c0 .239-.196.432-.439.432h-.995c-.231 0-.479.138-.532.473-.043.269.112.484.318.576l1.009.449c.221.098.318.354.22.572-.102.217-.361.314-.581.216l-1.068-.475c-.139-.063-.292-.029-.4.151-.446.735-1.31 1.464-2.532 1.6-1.246-.139-2.12-.894-2.557-1.643-.089-.152-.247-.164-.374-.107l-1.068.475c-.221.098-.479.001-.581-.216-.099-.218-.001-.474.22-.572l1.009-.449c.218-.097.359-.306.313-.609-.046-.285-.29-.44-.527-.44h-.996c-.243-.001-.439-.194-.439-.433s.196-.432.439-.432h1.001c.229 0 .47-.147.514-.462.034-.242-.114-.451-.322-.531l-.929-.362c-.226-.088-.337-.338-.248-.56.09-.222.345-.332.57-.244l.956.371c.123.047.298.032.392-.183.076-.169.16-.328.25-.477.713.416 1.497.624 2.377.624.867 0 1.661-.212 2.374-.625.095.154.183.318.259.494.081.183.249.221.386.168l.956-.371c.226-.088.48.021.57.244.089.222-.022.472-.248.56l-.929.361c-.2.077-.358.273-.32.556.038.274.272.438.512.438h1.001c.243-.001.439.192.439.431zm-6.587-4.847c.134.03.25.112.321.228.258.429-.078.796-.391 1.179.504.235 1.046.351 1.657.351.602 0 1.148-.119 1.654-.353-.308-.377-.644-.753-.389-1.177.071-.116.188-.198.321-.228.265-.058.465-.291.465-.573.001-.323-.265-.586-.595-.586-.393 0-.677.369-.575.74.038.141.018.29-.057.415-.169.279-.457.447-.824.449-.372-.002-.656-.173-.824-.449-.074-.125-.095-.274-.057-.415.102-.372-.182-.74-.575-.74-.33 0-.596.263-.596.586 0 .282.2.515.465.573z""/></svg>";
    const string cveNotSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M5.895 3.173c0-.648.534-1.173 1.192-1.173.785 0 1.355.736 1.15 1.48-.077.281-.035.58.116.829.334.553.903.895 1.647.898.733-.003 1.31-.34 1.647-.898.151-.25.193-.548.115-.829-.205-.743.364-1.48 1.151-1.48.658 0 1.192.525 1.192 1.173 0 .563-.402 1.029-.932 1.146-.268.059-.5.225-.64.457-.511.847.161 1.598.775 2.353-1.009.468-2.103.704-3.308.704-1.223 0-2.309-.231-3.312-.7.624-.767 1.296-1.502.779-2.358-.141-.232-.372-.397-.64-.457-.53-.116-.932-.582-.932-1.145zm4.105 12.827c0-3.327 2.042-6.184 4.939-7.389l-.189-.363c-1.429.827-3.017 1.252-4.75 1.252-1.761 0-3.329-.415-4.752-1.246-.181.299-.351.615-.5.954-.189.429-.539.46-.785.365l-1.913-.742c-.451-.176-.961.043-1.139.487-.178.444.044.946.495 1.121l1.86.722c.413.16.712.578.644 1.062-.088.631-.569.926-1.029.926h-2.003c-.485-.001-.878.386-.878.864 0 .477.393.864.878.864h1.989c.478 0 .966.31 1.055.88.093.607-.19 1.024-.626 1.218l-2.017.898c-.442.197-.638.709-.438 1.144s.721.628 1.162.431l2.134-.95c.256-.114.573-.088.75.216.871 1.497 2.62 3.009 5.113 3.286.783-.087 1.484-.306 2.117-.598-1.31-1.425-2.117-3.319-2.117-5.402zm14 0c0 3.314-2.686 6-6 6s-6-2.686-6-6 2.686-6 6-6 6 2.686 6 6zm-2.142 1h-1.858v-2h1.858c-.364-1.399-1.459-2.494-2.858-2.858v1.858h-2v-1.858c-1.399.364-2.494 1.459-2.858 2.858h1.858v2h-1.858c.364 1.399 1.459 2.494 2.858 2.858v-1.858h2v1.858c1.399-.364 2.494-1.459 2.858-2.858z""/></svg>";
    const string cveSubSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M13 9.406c0 .215-.177.389-.396.389h-.896c-.208 0-.431.125-.479.426-.038.241.102.435.287.519l.907.404c.199.088.287.318.198.514-.092.196-.325.283-.522.194l-.962-.427c-.125-.057-.263-.026-.36.136-.4.661-1.177 1.316-2.277 1.439-1.121-.125-1.908-.805-2.301-1.479-.08-.137-.223-.148-.337-.097l-.962.428c-.198.088-.431.001-.522-.194-.089-.196-.001-.426.198-.514l.907-.404c.196-.088.323-.275.282-.548-.04-.257-.26-.398-.474-.398h-.895c-.219 0-.396-.173-.396-.388 0-.216.177-.389.396-.389h.901c.206 0 .422-.132.462-.416.03-.217-.103-.406-.29-.478l-.836-.326c-.203-.079-.303-.304-.224-.504.081-.2.311-.298.514-.219l.86.334c.111.042.269.028.353-.165.068-.152.144-.294.224-.429.642.374 1.348.561 2.14.561.78 0 1.495-.191 2.137-.563.085.139.164.287.232.445.073.164.225.199.348.151l.86-.334c.203-.079.433.019.514.219.079.2-.021.425-.224.504l-.836.325c-.18.069-.322.246-.288.5.034.247.245.394.46.394h.901c.219.001.396.174.396.39zm-5.928-4.363c.12.027.225.101.289.205.232.386-.07.717-.351 1.062.452.211.94.315 1.49.315.541 0 1.033-.107 1.489-.318-.277-.339-.579-.677-.35-1.058.064-.104.169-.179.289-.206.238-.052.419-.262.419-.516 0-.29-.24-.527-.536-.527-.354 0-.61.332-.518.666.034.127.016.261-.051.373-.152.251-.412.402-.742.404-.335-.002-.591-.155-.742-.404-.067-.112-.085-.246-.051-.373.092-.334-.164-.666-.518-.666-.296 0-.536.237-.536.528 0 .253.181.463.419.515zm14.097 14.711c.522-.79.831-1.735.831-2.754 0-2.761-2.238-5-5-5s-5 2.239-5 5 2.238 5 5 5c1.019 0 1.964-.309 2.755-.832l2.831 2.832 1.414-1.414-2.831-2.832zm-4.169.246c-1.654 0-3-1.346-3-3s1.346-3 3-3 3 1.346 3 3-1.346 3-3 3zm-4.89 2h-7.11l2.599-3h2.696c.345 1.152.976 2.18 1.815 3zm-2.11-5h-10v-17h22v12.11c-.574-.586-1.251-1.068-2-1.425v-8.685h-18v13h8.295c-.19.634-.295 1.305-.295 2z""/></svg>";

    protected override async Task OnInitializedAsync()
    {
        breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["cveManagement"], href: "/cve",icon: cveSVG),
            new BreadcrumbItem(localizer["cveSubscriptions"], href: null, disabled: true,icon: cveSubSVG),
        };
        
        await LoadData();
    }

    private async Task LoadData()
    {
        loading = true;
        try
        {
            subscriptions = await CveController.GetUserSubscriptionsForComponentsAsync();
            userSummary = await CveController.GetUserSubscriptionSummaryForComponentsAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["errorLoadingSubscriptions"]}: {ex.Message}", Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private DialogOptionsEx largeWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = false,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Large,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { MudBlazor.Extensions.Options.AnimationType.SlideIn },
        Position = DialogPosition.Center,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };

    private async Task CreateSubscription()
    {
        var parameters = new DialogParameters();
        
        IMudExDialogReference<CveSubscriptionDialog>? dlgReference = await DialogService.ShowEx<CveSubscriptionDialog>(localizer["createSubscription"], parameters, largeWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await LoadData();
            Snackbar.Add(localizer["subscriptionCreatedSuccessfully"], Severity.Success);
        }
    }

    private async Task EditSubscription(CveSubscription subscription)
    {
        var parameters = new DialogParameters<CveSubscriptionDialog>
        {
            { x => x.ExistingSubscription, subscription }
        };
        
        IMudExDialogReference<CveSubscriptionDialog>? dlgReference = await DialogService.ShowEx<CveSubscriptionDialog>(localizer["editSubscription"], parameters, largeWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await LoadData();
            Snackbar.Add(localizer["subscriptionUpdatedSuccessfully"], Severity.Success);
        }
    }

    private async Task ToggleSubscription(CveSubscription subscription)
    {
        try
        {
            var success = await CveController.ToggleSubscriptionForComponentsAsync(subscription.Id);
            
            if (success)
            {
                if (subscription.IsActive)
                {
                    Snackbar.Add(localizer["subscriptionDeactivated"], Severity.Info);
                }
                else
                {
                    Snackbar.Add(localizer["subscriptionActivated"], Severity.Success);
                }
                await LoadData();
            }
            else
            {
                Snackbar.Add(localizer["errorTogglingSubscription"], Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["errorTogglingSubscription"]}: {ex.Message}", Severity.Error);
        }
    }

    private async Task DeleteSubscription(CveSubscription subscription)
    {
        var parameters = new DialogParameters<MudMessageBox>
        {
            { x => x.Message, localizer["confirmDeleteSubscriptionMessage"] },
            { x => x.YesText, localizer["delete"] },
            { x => x.NoText, localizer["cancel"] }
        };

        var dialog = await DialogService.ShowAsync<MudMessageBox>(localizer["deleteSubscription"], parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            try
            {
                var success = await CveController.DeleteSubscriptionForComponentsAsync(subscription.Id);
                
                if (success)
                {
                    await LoadData();
                    Snackbar.Add(localizer["subscriptionDeletedSuccessfully"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(localizer["errorDeletingSubscription"], Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"{localizer["errorDeletingSubscription"]}: {ex.Message}", Severity.Error);
            }
        }
    }

    private async Task ViewStatistics(CveSubscription subscription)
    {
        Navigation.NavigateTo($"/cve/subscriptions/{subscription.Id}/statistics");
    }

    private async Task CreateDefaultSubscriptions()
    {
        try
        {
            var createdSubscriptions = await CveController.CreateDefaultSubscriptionsForComponentsAsync();
            
            await LoadData(); // Refresh the subscription list
            
            Snackbar.Add(string.Format(localizer["defaultSubscriptionsCreatedSuccessfully"], createdSubscriptions.Count), Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["errorCreatingDefaultSubscriptions"]}: {ex.Message}", Severity.Error);
        }
    }

    private async Task TestSubscriptionNotification(CveSubscription subscription)
    {
        try
        {
            var result = await CveController.TestSubscriptionNotificationForComponentsAsync(subscription.Id);
            
            if (result.IsSuccess)
            {
                Snackbar.Add(string.Format(localizer["testNotificationCreated"], subscription.Name), Severity.Success);
            }
            else
            {
                Snackbar.Add($"{localizer["failedToCreateTestNotification"]}: {result.ErrorMessage}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["errorTestingNotification"]}: {ex.Message}", Severity.Error);
        }
    }

    private async Task TestAllActiveNotifications()
    {
        try
        {
            var results = await CveController.TestAllActiveSubscriptionsForComponentsAsync();
            
            var successCount = results.Count(r => r.IsSuccess);
            var failCount = results.Count(r => !r.IsSuccess);
            
            if (failCount == 0)
            {
                Snackbar.Add(string.Format(localizer["testNotificationsCreatedForAllSubscriptions"], successCount), Severity.Success);
            }
            else
            {
                Snackbar.Add(string.Format(localizer["testNotificationsPartialSuccess"], successCount, failCount), Severity.Warning);
            }

            // Show detailed results in a dialog
            await ShowTestResults(results);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["errorTestingNotifications"]}: {ex.Message}", Severity.Error);
        }
    }

    private async Task ShowTestResults(List<CveNotificationTestResult> results)
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true
        };

        var parameters = new DialogParameters<TestResultsDialog>
        {
            { x => x.Results, results }
        };

        await DialogService.ShowAsync<TestResultsDialog>(localizer["notificationTestResults"], parameters, options);
    }
}