using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CoreProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CoreProject
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/Login/Index";

                   });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //var connection = @"Server=az1-wsq1.a2hosting.com;Initial Catalog=mayankdb;Integrated Security=False;Persist Security Info=False;User ID=mayank;Password=Welcome@123456;ConnectRetryCount=0";
            //services.AddDbContext<mayankdbContext>(options => options.UseSqlServer(connection));
            //Get connection string from appsettings.json
            services.AddDbContext<mayankdbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAuthentication()
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["980167027207-gr2123m5ii9g55g4vnblstcolqjhqm23.apps.googleusercontent.com"];
                options.ClientSecret = googleAuthNSection["tgBCCsOGOeryTNVJ2dJQ4W_7"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
