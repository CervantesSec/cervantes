using System.Configuration;
using System.Security.Claims;
using AngleSharp;
using AuthPermissions;
using AuthPermissions.AspNetCore;
using AuthPermissions.AspNetCore.Services;
using AuthPermissions.AspNetCore.StartupServices;
using Blazored.LocalStorage;
using Cervantes.Application;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.DAL;
using Cervantes.IFR;
using Cervantes.IFR.CervantesAI;
using Cervantes.IFR.Email;
using Cervantes.IFR.Export;
using Cervantes.IFR.File;
using Cervantes.IFR.Jira;
using Cervantes.IFR.Parsers.Burp;
using Cervantes.IFR.Parsers.CSV;
using Cervantes.IFR.Parsers.Nessus;
using Cervantes.IFR.Parsers.Nmap;
using Cervantes.IFR.Parsers.Pwndoc;
using Cervantes.Server.Helpers;
using Cervantes.Web.AuthPermissions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cervantes.Web.Components;
using Cervantes.Web.Components.Account;
using Cervantes.Web.Controllers;
using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Antiforgery;
using MudBlazor.Services;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudExtensions.Services;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using MudExtensions.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;
using LogManager = Cervantes.Application.LogManager;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;
using RunMethodsSequentially;
using IndividualAccountUserLookup = Cervantes.Web.AuthPermissions.IndividualAccountUserLookup;
using SyncIndividualAccountUsers = Cervantes.Web.AuthPermissions.SyncIndividualAccountUsers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, o => {
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromMinutes(240);
    })
    .AddCookie(IdentityConstants.ExternalScheme);

if (builder.Configuration.GetValue<bool>("OpenIdConnect:Enabled"))
{
    builder.Services.AddAuthentication()
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = false;
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.Authority = builder.Configuration.GetSection("OpenIdConnect:Authority")?.Value;
            options.ClientId = builder.Configuration.GetSection("OpenIdConnect:ClientId")?.Value;
            options.ClientSecret = builder.Configuration.GetSection("OpenIdConnect:ClientSecret")?.Value;
            options.CallbackPath = new PathString("/Account/ExternalLogin");
            options.SignedOutCallbackPath = new PathString("/Account/Logout");
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SkipUnrecognizedRequests = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Name, "email");
            options.Scope.Add("email");
        });
}

builder.Services.ConfigureApplicationCookie(options =>
{
    //this will cause all the logged-in users to have their claims periodically updated
    options.Events.OnValidatePrincipal = PeriodicCookieEvent.PeriodicRefreshUsersClaims;
});

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseVector()), ServiceLifetime.Scoped);
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.RegisterAuthPermissions<Permissions>(options =>
    {
        options.PathToFolderToLock = builder.Environment.WebRootPath;
    })
    .UsingEfCorePostgres(connectionString)
    .IndividualAccountsAuthentication<ApplicationUser>()
    .AddRolesPermissionsIfEmpty(AppAuthSetupData.RolesDefinition)
    .AddAuthUsersIfEmpty(AppAuthSetupData.UsersWithRolesDefinition)
    .RegisterAuthenticationProviderReader<SyncIndividualAccountUsers>()
    .RegisterFindUserInfoService<IndividualAccountUserLookup>()
    .SetupAspNetCoreAndDatabase(options =>
    {
        //Migrate individual account database
        options.RegisterServiceToRunInJob<StartupServiceMigrateAnyDbContext<ApplicationDbContext>>();
    });

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddNLog(new NLogProviderOptions {
    CaptureMessageTemplates = true,
    CaptureMessageProperties = true
});
builder.Host.UseNLog();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;

});

builder.Services.AddControllers();

/*builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
}).AddCircuitOptions(options => { options.DetailedErrors = true; });*/

builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

builder.Services.AddMudServices();
MudExtensions.Services.ExtensionServiceCollectionExtensions.AddMudExtensions(builder.Services);
MudBlazor.Extensions.ServiceCollectionExtensions.AddMudExtensions(builder.Services,c => c.WithoutAutomaticCssLoading());
builder.Services.AddLocalization();
builder.Services.AddAntiforgery();
builder.Services.AddBlazoredLocalStorage();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<IClientManager, ClientManager>();
builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<IOrganizationManager, OrganizationManager>();
builder.Services.AddScoped<IProjectUserManager, ProjectUserManager>();
builder.Services.AddScoped<IProjectNoteManager, ProjectNoteManager>();
builder.Services.AddScoped<IProjectAttachmentManager, ProjectAttachmentManager>();
builder.Services.AddScoped<ITargetManager, TargetManager>();
builder.Services.AddScoped<ITargetServicesManager, TargetServicesManager>();
builder.Services.AddScoped<ITaskManager, TaskManager>();
builder.Services.AddScoped<ITaskNoteManager, TaskNoteManager>();
builder.Services.AddScoped<ITaskTargetManager, TaskTargetManager>();
builder.Services.AddScoped<ITaskAttachmentManager, TaskAttachmentManager>();
builder.Services.AddScoped<IVulnManager, VulnManager>();
builder.Services.AddScoped<IVulnCategoryManager, VulnCategoryManager>();
builder.Services.AddScoped<IVulnNoteManager, VulnNoteManager>();
builder.Services.AddScoped<IVulnAttachmentManager, VulnAttachmentManager>();
builder.Services.AddScoped<IVulnTargetManager, VulnTargetManager>();
builder.Services.AddScoped<IDocumentManager, DocumentManager>();
builder.Services.AddScoped<INoteManager, NoteManager>();
builder.Services.AddScoped<ILogManager, Cervantes.Application.LogManager>();
builder.Services.AddScoped<IReportManager, ReportManager>();
builder.Services.AddScoped<IReportTemplateManager, ReportTemplateManager>();
builder.Services.AddScoped<IVaultManager, VaultManager>();
builder.Services.AddScoped<INmapParser, NmapParser>();
builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IJiraConfiguration>(builder.Configuration.GetSection("JiraConfiguration").Get<JiraConfiguration>());
builder.Services.AddScoped<IFileCheck, FileCheck>();
builder.Services.AddScoped<IJIraService, JiraService>();
builder.Services.AddScoped<IJiraManager, JiraManager>();
builder.Services.AddScoped<IJiraCommentManager, JiraCommentManager>();
builder.Services.AddScoped<ICsvParser, CsvParser>();
builder.Services.AddScoped<IPwndocParser, PwndocParser>();
builder.Services.AddScoped<IWSTGManager, WSTGManager>();
builder.Services.AddScoped<IMASTGManager, MASTGManager>();
builder.Services.AddScoped<IBurpParser, BurpParser>();
builder.Services.AddScoped<INessusParser, NessusParser>();
builder.Services.AddScoped<ICweManager, CweManager>();
builder.Services.AddScoped<IVulnCweManager, VulnCweManager>();
builder.Services.AddTransient<IChatManager, ChatManager>();
builder.Services.AddSingleton<IExportToCsv,ExportToCsv>();
builder.Services.AddScoped<IKnowledgeBaseManager, KnowledgeBaseManager>();
builder.Services.AddScoped<IKnowledgeBaseCategoryManager, KnowledgeBaseCategoryManager>();
builder.Services.AddTransient<IKnowledgeBaseTagManager, KnowledgeBaseTagManager>();
builder.Services.AddScoped<IReportComponentsManager, ReportComponentsManager>();
builder.Services.AddScoped<IReportsPartsManager, ReportPartsManager>();
builder.Services.AddSingleton<IAiConfiguration>(builder.Configuration.GetSection("AIConfiguration").Get<AiConfiguration>());
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IChatManager, ChatManager>();
builder.Services.AddScoped<IChatMessageManager, ChatMessageManager>();
builder.Services.AddScoped<IRssNewsManager, RssNewsManager>();
builder.Services.AddScoped<IRssSourceManager, RssSourceManager>();

builder.Services.AddScoped<Sanitizer>();
builder.Services.AddScoped<ClientsController>();
builder.Services.AddScoped<ProjectController>();
builder.Services.AddScoped<VulnController>();
builder.Services.AddScoped<UserController>();
builder.Services.AddScoped<OrganizationController>();
builder.Services.AddScoped<ReportController>();
builder.Services.AddScoped<BackupController>();
builder.Services.AddScoped<DocumentController>();
builder.Services.AddScoped<LogController>();
builder.Services.AddScoped<NoteController>();
builder.Services.AddScoped<TargetController>();
builder.Services.AddScoped<TaskController>();
builder.Services.AddScoped<WorkspacesController>();
builder.Services.AddScoped<ChecklistController>();
builder.Services.AddScoped<KnowledgeBaseController>();
builder.Services.AddScoped<CalendarController>();
builder.Services.AddScoped<VaultController>();
builder.Services.AddScoped<SearchController>();
builder.Services.AddScoped<JiraController>();
builder.Services.AddScoped<ChatController>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cervantes API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMudExtensions();

app.UseHttpsRedirection();

app.UseStaticFiles();
var supportedCultures = new[] { "en-US", "es-ES","pt-PT" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
var options = new DashboardOptions
{
    DashboardTitle = "Cervantes Jobs",
    Authorization = new[] { new HangfireAuthorizationFilter() }
};

app.UseHangfireDashboard("/jobs", options);


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var vulnCategoryManager = serviceProvider.GetRequiredService<IVulnCategoryManager>();
        var organizationManager = serviceProvider.GetRequiredService<IOrganizationManager>();
        var reportTemplateManager = serviceProvider.GetRequiredService<IReportTemplateManager>();
        var cweManager = serviceProvider.GetRequiredService<ICweManager>();
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var reportComponentsManager = serviceProvider.GetRequiredService<IReportComponentsManager>();
        var reportPartsManager = serviceProvider.GetRequiredService<IReportsPartsManager>();
        DataInitializer.SeedData(userManager, roleManager, vulnCategoryManager, organizationManager,reportTemplateManager,cweManager, env.WebRootPath+"/Resources/Data/cwe.xml", reportComponentsManager, reportPartsManager);
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Clients") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Clients");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Documents") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Documents");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Export") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Export");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Projects") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Projects");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Reports") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Reports");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Task") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Task");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Temp") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Temp");
        }
        
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Vulns") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Vulns");
        }
        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Imports") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Imports");
        }
    }
    catch (Exception ex)
    {
        throw;
    }
}


// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

var time = builder.Configuration.GetSection("Logging:TimeArchive")?.Value;

if (string.IsNullOrEmpty(time))
{
    time = "0 0 */30 * *";
}


using (var scope = app.Services.CreateScope())
{
    var _LogManager = scope.ServiceProvider.GetRequiredService<Cervantes.Contracts.ILogManager>();

    RecurringJob.AddOrUpdate(
        "DeleteOldLogs",
        () => _LogManager.DeleteAllAsync(),
        time.ToString());
}


app.Run();