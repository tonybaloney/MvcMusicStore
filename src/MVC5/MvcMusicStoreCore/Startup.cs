using MvcMusicStoreCore.Extensions;
using MvcMusicStoreCore.Models;

namespace MvcMusicStoreCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSystemWebAdapters();
            services.AddControllersWithViews();
            services.AddScoped<MusicStoreEntities>(_ => new MusicStoreEntities(Configuration.GetConnectionString("MusicStoreEntities")));
            services.AddTransient<SampleData>();
            services.AddScoped<AIFunctionApiClient>(_ => new AIFunctionApiClient(Configuration.GetConnectionString("AIFunctionEndpoint")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else { 
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseSystemWebAdapters();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
