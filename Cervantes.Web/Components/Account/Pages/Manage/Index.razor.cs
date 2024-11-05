using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.DAL;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Net.Codecrete.QrCodeGenerator;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Account.Pages.Manage;

public partial class Index: ComponentBase
{
    private ApplicationUser user = default!;
    private string? username;
    private string? phoneNumber;
    private List<BreadcrumbItem> _items;
    [Inject] private UserController _UserController { get; set; }

    private ILogger<UserController> Logger { get; set; }
    [CascadingParameter] private HttpContext HttpContext { get; set; } = default!;
    private ProfileEdit modelEdit { get; set; } = new ProfileEdit();
    MudForm formEdit;
    MudForm formChange;
    MudForm form;

    public string QRByte = "";
    private string Role = "";
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
    
    protected override async Task OnInitializedAsync()
    {
        //user = userController.GetUser(_accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        user = await UserManager.GetUserAsync(_accessor.HttpContext.User);

        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/", icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["profile"], href: null, disabled: true, icon: Icons.Material.Filled.Person)
        };
        
        modelEdit.Id = user.Id;
        modelEdit.FullName = user.FullName;
        modelEdit.PhoneNumber = user.PhoneNumber;
        modelEdit.Position = user.Position;
        
        
        await LoadSharedKeyAndQrCodeUriAsync();
        twoFactorModel = new TwoFactorModel();
        
        emailModel.NewEmail = user.Email;
        var rolUser = await _UserController.GetRole(user.Id);
        Role = rolUser;
    }
  
    
    private string GetAntiforgeryToken()
    {
        return Antiforgery.GetAndStoreTokens(_accessor.HttpContext).RequestToken;
    }

    #region Profile
    private static IBrowserFile File;
    private long maxFileSize = 1024 * 1024 * 5;
    private ProfileUploadAvatar avatar { get; set; } = new ProfileUploadAvatar();
    private async Task UploadFile(IBrowserFile file)
    {

            avatar = new ProfileUploadAvatar();
            Stream stream = file.OpenReadStream(maxFileSize);
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            stream.Close();
	        avatar.Id = user.Id;
            avatar.FileName = file.Name;
            avatar.FileContent = ms.ToArray();
            ms.Close();
        
        var response = await _UserController.UploadAvatar(avatar);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["avatarUploaded"], Severity.Success);
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["avatarUploadedError"], Severity.Error);
        }
    }
    private async Task DeleteAvatar(string id)
    {
        var response = await _UserController.DeleteAvatar(id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["logoDeleted"], Severity.Success);
            user.Avatar = null;
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["logoDeletedError"], Severity.Error);
        }

    }
    
    UserModelFluentValidator userModelFluentValidator = new UserModelFluentValidator();

    public class UserModelFluentValidator : AbstractValidator<ProfileEdit>
    {
        public UserModelFluentValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(1,100);
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ProfileEdit>.CreateWithOptions((ProfileEdit)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task SubmitProfile()
    {
        await formEdit.Validate();

        if (formEdit.IsValid)
        {

            var response = await _UserController.ProfileEdit(modelEdit);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(localizer["profileEdited"], Severity.Success);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add(localizer["profileEditedError"], Severity.Error);
                StateHasChanged();
            }
            
        }
    }
    #endregion
    
    #region PersonalData
    private async Task DeletePersonalData()
    {
        var result = await userController.Delete(user.Id);
        await SignInManager.SignOutAsync();

        Logger.LogInformation("User with ID '{UserId}' deleted themselves.", user.Id);
        RedirectManager.RedirectTo("/Account/Login");
    }
    

    #endregion
   
    #region TwoFactorAuthentication
    
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private string? sharedKey ="";
    private string? authenticatorUri = "";
    private IEnumerable<string>? recoveryCodes;
    private TwoFactorModel twoFactorModel { get; set; }

    private async Task TwoFactorSubmit()
    {
        try
        {
            recoveryCodes = new [] { "" };

                var verificationCode = twoFactorModel.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
        
                if (is2faTokenValid == false)
                {
                    Snackbar.Add("twoFactorSetError", Severity.Error);
                    return;
                }

                await UserManager.SetTwoFactorEnabledAsync(user, true);
                //Logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);

                Snackbar.Add("twoFactorSetted", Severity.Success);
                user = userController.GetUser(user.Id);
                if (await UserManager.CountRecoveryCodesAsync(user) == 0)
                {
                    recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                }
                StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
    }
    private async ValueTask LoadSharedKeyAndQrCodeUriAsync()
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await UserManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        }

        sharedKey = FormatKey(unformattedKey!);

        //var email = await UserManager.GetEmailAsync(user);
        authenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey!);

        var qr = QrCode.EncodeText(authenticatorUri, QrCode.Ecc.Medium);
        var bmp = qr.ToBmpBitmap(4);
        string base64 = Convert.ToBase64String(bmp);
        QRByte = string.Format("data:image/bmp;base64,{0}", base64);
    }
    
    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            UrlEncoder.Encode("Cervantes"),
            UrlEncoder.Encode(email),
            unformattedKey);
    }
    
    private sealed class TwoFactorModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = "";
    }
    
    private async Task Disable2faSubmit()
    {
        var disable2faResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            Snackbar.Add("disabled2faError", Severity.Error);
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        var userId = await UserManager.GetUserIdAsync(user);
        Logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userId);
        Snackbar.Add("disabled2fa", Severity.Success);
        
        /*RedirectManager.RedirectToWithStatus(
            "Account/Manage/TwoFactorAuthentication",
            "2fa has been disabled. You can reenable 2fa when you setup an authenticator app",
            HttpContext);*/
        user = await UserManager.GetUserAsync(_accessor.HttpContext.User);
        StateHasChanged();
    }
    
    private async Task ResetAuthenticatorSubmit()
    {
        await UserManager.SetTwoFactorEnabledAsync(user, false);
        await UserManager.ResetAuthenticatorKeyAsync(user);
        var userId = await UserManager.GetUserIdAsync(user);
        
        Logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", userId);

        await SignInManager.RefreshSignInAsync(user);
        Snackbar.Add("resetedAuthenticator", Severity.Success);

        /*RedirectManager.RedirectToWithStatus(
            "Account/Manage/EnableAuthenticator",
            "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.",
            HttpContext);*/
    }
    
    #endregion
    
    #region Change Password
    
    ChangePasswordFluentValidator changeFluentValidator = new ChangePasswordFluentValidator();

    public class ChangePasswordFluentValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordFluentValidator()
        {
            RuleFor(p => p.OldPassword).NotEmpty();
            RuleFor(p => p.NewPassword).NotEmpty()
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChangePasswordModel>.CreateWithOptions((ChangePasswordModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private ChangePasswordModel changePasswordModel { get; set; } = new();
    public class ChangePasswordModel
    {
        
        public string OldPassword { get; set; } = "";
        
        public string NewPassword { get; set; } = "";

    }
    
    private async Task SubmitPasswordAsync()
    {
        var changePasswordResult = await UserManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            Snackbar.Add(@localizer["passwordChangedError"], Severity.Error);
            return;
        }
        Snackbar.Add(@localizer["passwordChanged"], Severity.Success);

        await SignInManager.RefreshSignInAsync(user);
        Logger.LogInformation("User changed their password successfully.");
        RedirectManager.RedirectToCurrentPageWithStatus("Your password has been changed", HttpContext);
    }
    #endregion

    #region Email
    EmailModel emailModel { get; set; } = new();
    
    private async Task EmailSubmit()
    {
        if (emailModel.NewEmail is null || emailModel.NewEmail == user.Email)
        {
            Snackbar.Add(@localizer["sameEmail"], Severity.Warning);
            return;
        }

        await UserManager.SetEmailAsync(user, emailModel.NewEmail);
        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        await UserManager.ConfirmEmailAsync(user, token);
        user.UserName = emailModel.NewEmail;
        user.NormalizedUserName = emailModel.NewEmail.ToUpper();
        await UserManager.UpdateAsync(user);
        Snackbar.Add(@localizer["emailChanged"], Severity.Success);

        StateHasChanged();
    }
    private sealed class EmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }
    }
    

    #endregion
    
    
    

}