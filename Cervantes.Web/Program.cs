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
using Cervantes.Server.Helpers;
using Cervantes.Web.AuthPermissions;
using Cervantes.Web.Extensions;
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
using Task = System.Threading.Tasks.Task;

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
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SkipUnrecognizedRequests = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Name, "email");
            options.Scope.Add("email");
        });
}

/*builder.Services.ConfigureApplicationCookie(options =>
{
    //this will cause all the logged-in users to have their claims periodically updated
    options.Events.OnValidatePrincipal = PeriodicCookieEvent.PeriodicRefreshUsersClaims;
});*/

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnValidatePrincipal = PeriodicCookieEvent.PeriodicRefreshUsersClaims;
    options.Events.OnRedirectToLogin = context =>
    {
        // Check if the request is for the API
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        // For non-API requests, continue with the redirect
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    // Also handle other redirects
    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
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
//builder.Services.AddMudExtensions(c => c.WithoutAutomaticCssLoading());
MudExtensions.Services.ExtensionServiceCollectionExtensions.AddMudExtensions(builder.Services);
MudBlazor.Extensions.ServiceCollectionExtensions.AddMudExtensions(builder.Services,c => c.WithoutAutomaticCssLoading());
builder.Services.AddLocalization();
builder.Services.AddAntiforgery();
builder.Services.AddBlazoredLocalStorage();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

// Register all managers
builder.Services.AddCervantesManagers();

// Register all vulnerability parsers
builder.Services.AddVulnerabilityParsers();

// Register all external services
builder.Services.AddExternalServices(builder.Configuration);
// Register all controllers
builder.Services.AddCervantesControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cervantes API", Version = "v1" });
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Enter your username and password as Base64 encoded value in the format 'username:password'."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();
app.Use(MudExWebApp.MudExMiddleware);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

app.UseHttpsRedirection();

app.UseStaticFiles();
var supportedCultures = new[] { "en-US", "es-ES","pt-PT", "tr-TR", "fr-FR", "de-DE", "it-IT", "cs-CZ" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseRouting();
app.UseBasicAuthForApi();
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
