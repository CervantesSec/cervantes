using Cervantes.Application;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.DAL;
using Cervantes.Web.LocalizationResources;
using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.InteropServices;
using Wkhtmltopdf.NetCore;

namespace Cervantes.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add the localization services to the services container
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });

            services.AddRazorPages();



            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies().UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IRoleManager, RoleManager>();
            services.AddScoped<IClientManager, ClientManager>();
            services.AddScoped<IProjectManager, ProjectManager>();
            services.AddScoped<IOrganizationManager, OrganizationManager>();
            services.AddScoped<IProjectUserManager, ProjectUserManager>();
            services.AddScoped<IProjectNoteManager, ProjectNoteManager>();
            services.AddScoped<IProjectAttachmentManager, ProjectAttachmentManager>();
            services.AddScoped<ITargetManager, TargetManager>();
            services.AddScoped<ITargetServicesManager, TargetServicesManager>();
            services.AddScoped<ITaskManager, TaskManager>();
            services.AddScoped<ITaskNoteManager, TaskNoteManager>();
            services.AddScoped<ITaskAttachmentManager, TaskAttachmentManager>();
            services.AddScoped<IVulnManager, VulnManager>();
            services.AddScoped<IVulnCategoryManager, VulnCategoryManager>();
            services.AddScoped<IVulnNoteManager, VulnNoteManager>();
            services.AddScoped<IVulnAttachmentManager, VulnAttachmentManager>();
            services.AddScoped<IDocumentManager, DocumentManager>();
            services.AddScoped<INoteManager, NoteManager>();
            services.AddScoped<ILogManager, LogManager>();
            services.AddScoped<IReportManager, ReportManager>();
            services.AddScoped<INotificationsManager, NotificationsManager>();



            var cultures = new[]
             {
                new CultureInfo("en"),
                new CultureInfo("es"),
            };



            services.AddControllersWithViews()
                .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(ops =>
                {

                    ops.UseAllCultureProviders = false;
                    ops.ResourcesPath = "LocalizationResources";
                    ops.RequestLocalizationOptions = o =>
                    {
                        o.SupportedCultures = cultures;
                        o.SupportedUICultures = cultures;
                        o.DefaultRequestCulture = new RequestCulture("en");
                    };
                }); ;
            
            //services.AddWkhtmltopdf();

        }
        
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                 name: "Workspace",
                 areaName: "Workspace",
                 pattern: "{culture=en}/Workspace/{project}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=en}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();


            });
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Rotativa.AspNetCore.RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env, "Rotativa\\Windows\\");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Rotativa.AspNetCore.RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env, "Rotativa/MacOS/");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Rotativa.AspNetCore.RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env, "Rotativa/Linux/");
            }
            
        }
    }
}
