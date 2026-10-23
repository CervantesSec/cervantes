using Microsoft.AspNetCore.HttpOverrides;
using System.Configuration;
using System.Security.Claims;
using System.Text.Json.Serialization.Metadata;
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
using Cervantes.IFR.ChecklistMigration;
using Cervantes.Server.Helpers;
using Cervantes.Web.AuthPermissions;
using Cervantes.Web.Authentication;
using Cervantes.Web.Extensions;
using Cervantes.CORE.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cervantes.Web.Components;
using Cervantes.Web.Components.Account;
using Cervantes.Web.Controllers;
using Cervantes.Web.Helpers;
using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.RateLimiting;
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
using MudBlazor;
using RunMethodsSequentially;
using IndividualAccountUserLookup = Cervantes.Web.AuthPermissions.IndividualAccountUserLookup;
using SyncIndividualAccountUsers = Cervantes.Web.AuthPermissions.SyncIndividualAccountUsers;
using Task = System.Threading.Tasks.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Autosave local storage service
builder.Services.AddScoped<Cervantes.Web.Helpers.IAutosaveStorage, Cervantes.Web.Helpers.LocalStorageAutosaveStorage>();
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
    .AddCookie(IdentityConstants.ExternalScheme)
    // The two-factor cookies, as AddIdentityCookies registers them. The login stores the half-signed-in user in the
    // user id cookie between the password and the code (without it a two-factor user was sent back to the login), and
    // "remember this machine" uses the remember-me cookie, which a password change must end. SecurityStampValidator
    // also signs the remember-me scheme out when it rejects an application cookie.
    .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
    {
        o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
        o.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
        };
    })
    .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
    {
        o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
        o.Events = new CookieAuthenticationEvents { OnRedirectToReturnUrl = _ => Task.CompletedTask };
        o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });

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

// LDAP Authentication
if (builder.Configuration.GetValue<bool>("LdapConfiguration:Enabled"))
{
    builder.Services.AddAuthentication()
        .AddScheme<LdapAuthenticationOptions, LdapAuthenticationHandler>(
            LdapDefaults.AuthenticationScheme,
            LdapDefaults.DisplayName,
            options =>
            {
                options.CallbackPath = new PathString("/Account/ExternalLogin");
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
    
    // Register the LDAP authentication handler as a service
    builder.Services.AddScoped<LdapAuthenticationHandler>();
}

/*builder.Services.ConfigureApplicationCookie(options =>
{
    //this will cause all the logged-in users to have their claims periodically updated
    options.Events.OnValidatePrincipal = PeriodicCookieEvent.PeriodicRefreshUsersClaims;
});*/

// Sessions end within this interval of a lock, a delete or an edit that changes how the user signs in (all of which
// change the security stamp). The interactive circuit revalidates on the same interval.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.FromMinutes(builder.Configuration.GetValue("SecurityStamp:ValidationIntervalMinutes", 30)));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<RefreshSignInTickets>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // AddIdentityCore registers the security stamp validator but does not hook it into the cookie, and this event
    // replaces the handler anyway, so it runs the validator first. Only a principal that survives it gets its AuthP
    // claims refreshed.
    options.Events.OnValidatePrincipal = async context =>
    {
        await SecurityStampValidator.ValidatePrincipalAsync(context);
        if (context.Principal?.Identity?.IsAuthenticated == true)
        {
            await PeriodicCookieEvent.PeriodicRefreshUsersClaims(context);
        }
    };
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
builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("ApiKeys"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseVector()), ServiceLifetime.Scoped);
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
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

// API responses never carry user credential fields (see UserJsonModifiers). Backups serialize separately.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { UserJsonModifiers.HideUserSecrets }
    });

/*builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
}).AddCircuitOptions(options => { options.DetailedErrors = true; });*/

builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

builder.Services.AddMudServices();
//builder.Services.AddMudExtensions(c => c.WithoutAutomaticCssLoading());
MudExtensions.Services.ExtensionServiceCollectionExtensions.AddMudExtensions(builder.Services);
MudBlazor.Extensions.ServiceCollectionExtensions.AddMudExtensions(builder.Services,c => c.WithoutAutomaticCssLoading());
builder.Services.AddTransient<MudLocalizer, ResXMudLocalizer>();
builder.Services.AddLocalization();

// Behind a reverse proxy (TLS offload), X-Forwarded-For/-Proto give the client IP (audit logs) and the original scheme
// (redirect URLs, OpenID Connect callbacks). Off by default: the headers are only honoured from the proxies listed
// here, plus loopback, so a client talking to the app directly cannot spoof them. X-Forwarded-Host is never trusted.
var forwardedHeadersEnabled = builder.Configuration.GetValue<bool>("ForwardedHeaders:Enabled");
if (forwardedHeadersEnabled)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        foreach (var proxy in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
        {
            options.KnownProxies.Add(System.Net.IPAddress.Parse(proxy));
        }
        foreach (var network in builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ?? Array.Empty<string>())
        {
            options.KnownNetworks.Add(Microsoft.AspNetCore.HttpOverrides.IPNetwork.Parse(network));
        }
    });
}
builder.Services.AddAntiforgery();

// Optional CSP violation reporting (off by default): /csp-report is anonymous, so it is rate-limited per client IP.
var cspReportingEnabled = builder.Configuration.GetValue<bool>("CspReporting:Enabled");
if (cspReportingEnabled)
{
    var cspReportPermitPerMinute = builder.Configuration.GetValue("CspReporting:PermitPerMinute", 30);
    if (cspReportPermitPerMinute < 1)
    {
        cspReportPermitPerMinute = 30;
    }
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        // Per-client limit, applied by the rate limiter middleware before the handler runs. The overall cap comes
        // after it, in the handler, so a client already rejected here does not use up the shared window.
        options.AddPolicy("csp-report", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            CspReports.PartitionKey(httpContext.Connection.RemoteIpAddress),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = cspReportPermitPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
    });
    // Overall cap for the endpoint, shared by all clients: a flood spread over many networks cannot fill the Logs table.
    builder.Services.AddSingleton(new CspReportGlobalLimiter(cspReportPermitPerMinute));
}
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
    
    // Add this to resolve conflicts with duplicate schema IDs
    c.CustomSchemaIds(type => type.FullName);
    
    // Enable annotations
    c.EnableAnnotations();
    
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
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = builder.Configuration.GetValue<string>("ApiKeys:HeaderName") ?? "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Provide an API key in the configured header (default X-API-Key). Example: ck_abcd1234.<secret>"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] { }
        }
    });
    
    // Add filter to handle exceptions
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

if (forwardedHeadersEnabled)
{
    // First in the pipeline, so everything after it sees the client's IP and scheme.
    app.UseForwardedHeaders();
}

// PreMailer (the DOCX report export, which inlines the stored report) downloads <link> stylesheets through this process-wide downloader;
// exports must not fetch anything, so it gets one that returns no CSS.
PreMailer.Net.Downloaders.WebDownloader.SharedDownloader = new OfflineCssDownloader();

// Security headers (CSP, nosniff, Referrer-Policy): see SecurityHeaders for what each directive allows and why.
// With OpenID Connect, the provider's origin is added to form-action.
string? openIdAuthorityOrigin = null;
IReadOnlyList<string> openIdFormActionOrigins = Array.Empty<string>();
if (app.Configuration.GetValue<bool>("OpenIdConnect:Enabled"))
{
    var authorityEntry = app.Configuration["OpenIdConnect:Authority"];
    var parsedAuthority = SecurityHeaders.ParseOrigins(new[] { authorityEntry });
    openIdAuthorityOrigin = parsedAuthority.Origins.FirstOrDefault();
    if (openIdAuthorityOrigin == null)
    {
        app.Logger.LogWarning("OpenIdConnect:Authority is not an absolute http(s) URL, it is not added to form-action: {Entry}", authorityEntry);
    }
    var parsedOrigins = SecurityHeaders.ParseOrigins(app.Configuration.GetSection("OpenIdConnect:FormActionOrigins").Get<string[]>());
    foreach (var entry in parsedOrigins.Invalid)
    {
        app.Logger.LogWarning("OpenIdConnect:FormActionOrigins entry ignored, it is not an absolute http(s) URL: {Entry}", entry);
    }
    openIdFormActionOrigins = parsedOrigins.Origins;
}
var contentSecurityPolicy = SecurityHeaders.ContentSecurityPolicy(openIdAuthorityOrigin, openIdFormActionOrigins,
    cspReportingEnabled ? CspReports.Path : null);
app.Use((context, next) =>
{
    // OnStarting, not a plain assignment: UseExceptionHandler clears the response (headers included) before it
    // re-executes /Error, and this middleware sits before it, so a header set up front is lost on error pages.
    context.Response.OnStarting(state =>
    {
        var httpContext = (HttpContext)state;
        SecurityHeaders.Apply(httpContext.Response.Headers, contentSecurityPolicy,
            httpContext.Items.ContainsKey(SecurityHeaders.SandboxItem));
        return Task.CompletedTask;
    }, context);
    return next();
});

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

// Attachments are not served statically: they go through the authenticated FileController. The provider refuses any
// file whose resolved path is inside wwwroot/Attachments, whatever the URL looks like.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new AttachmentExcludingFileProvider(app.Environment.WebRootFileProvider, app.Environment.WebRootPath)
});
var supportedCultures = SupportedCultures.Names;
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
app.UseRouting();
// After UseRouting: the endpoint-specific "csp-report" policy needs the selected endpoint.
if (cspReportingEnabled)
{
    app.UseRateLimiter();
}
app.UseApiKeyAuthForApi();
app.UseBasicAuthForApi();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

if (cspReportingEnabled)
{
    // Always 204: the reporter must not learn what was kept. Reports are logged as warnings, not stored elsewhere.
    app.MapPost(CspReports.Path, async (HttpContext httpContext, ILoggerFactory loggerFactory, CspReportGlobalLimiter globalLimiter) =>
    {
        // Order: per-client limit (middleware policy, already passed), then the overall cap here.
        using var lease = globalLimiter.AttemptAcquire();
        if (!lease.IsAcquired)
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        }
        try
        {
            var body = await CspReports.ReadLimitedAsync(httpContext.Request.Body, CspReports.MaxBodyBytes, httpContext.RequestAborted);
            var violation = body == null ? null : CspReports.Parse(body);
            if (violation != null && !CspReports.IsIgnored(violation))
            {
                loggerFactory.CreateLogger("Cervantes.Web.CspReports").LogWarning(
                    "CSP violation: {Directive} blocked {BlockedUri} on {DocumentPath} ({SourceFile}:{Line})",
                    violation.Directive, violation.BlockedUri, violation.DocumentPath, violation.SourceFile, violation.Line);
            }
        }
        catch (Exception)
        {
            // A bad or aborted report is not worth a 500 and a stack trace in Logs.
        }
        return Results.NoContent();
    }).AllowAnonymous().DisableAntiforgery().RequireRateLimiting("csp-report");
}

// Map SignalR Hub
app.MapHub<Cervantes.Web.Hubs.CveNotificationHub>("/cvehub");
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
        var checklistMigrationService = serviceProvider.GetRequiredService<ChecklistMigrationService>();
        DataInitializer.SeedData(userManager, roleManager, vulnCategoryManager, organizationManager,reportTemplateManager,cweManager, env.WebRootPath+"/Resources/Data/cwe.xml", reportComponentsManager, reportPartsManager);
        
        // Initialize system checklist templates (WSTG/MASTG)
        var systemUserId = userManager.Users.FirstOrDefault(u => u.Email == "admin@cervantes.local")?.Id;
        if (!string.IsNullOrEmpty(systemUserId))
        {
            checklistMigrationService.InitializeSystemTemplates(systemUserId).Wait();
        }
        
        //Check folders
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
        // Older versions used this folder for backup uploads (<guid>.json and <guid>.zip); nothing writes here any
        // more, so those files are cleaned up. Other files, such as the folder's .gitignore, are kept.
        foreach (var leftover in System.IO.Directory.EnumerateFiles($"{env.WebRootPath}/Attachments/Temp")
                     .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                                 || f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                System.IO.File.Delete(leftover);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                app.Logger.LogWarning(e, "Could not delete the leftover backup upload {File}", leftover);
            }
        }

        if (System.IO.Directory.Exists($"{env.WebRootPath}/Attachments/Vuln") == false)
        {
            System.IO.Directory.CreateDirectory($"{env.WebRootPath}/Attachments/Vuln");
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
