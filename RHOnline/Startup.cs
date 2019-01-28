using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using RHOnline.Database;
using Rotativa.AspNetCore;

namespace RHOnline
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


            services.AddDbContext<DatabaseContext>(options =>
            {
                //options.UseSqlServer("Driver={SQL Server Native Client 10.0};Server=tcp:lz72uivirk.database.windows.net,1433;Database=ELETRO_AZURE;Uid=eletro_az@lz72uivirk;Pwd=if360alWK;Encrypt=yes;Connection Timeout=30;");
                options.UseSqlServer("Server=tcp:lz72uivirk.database.windows.net,1433;Database=ELETRO_AZURE;Uid=eletro_az@lz72uivirk;Pwd=if360alWK;Encrypt=yes;Connection Timeout=30;");
                options.EnableSensitiveDataLogging(true);

            });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1); //Adiciona o servi�o do MVC
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddProgressiveWebApp();
            services.AddServiceWorker();
            services.AddAuthentication();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
                options.RequestCultureProviders.Clear();
            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //Se estiver no ambiente de desenvolvimento
                app.UseDeveloperExceptionPage();
            }
            else
            { //Se estiver no ambiente de Produ��o

                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error"); //Redireciona para a p�gina de Erro
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //Usa os arquivos est�ticos como imagens e documentos
            app.UseCookiePolicy();

            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseMvc(routes =>
                { //Habilita o uso do MVC

                    routes.MapRoute(
                            name: "default",
                            template: "/RHOnline/{controller=Home}/{action=Index}/{id?}");
                });
            }
            else
            {
                app.UseMvc(routes =>
                { //Habilita o uso do MVC

                    routes.MapRoute(
                            name: "default",
                            template: "{controller=Home}/{action=Index}/{id?}");
                });
            }

            app.UseRequestLocalization();

            RotativaConfiguration.Setup(env);
        }
    }
}