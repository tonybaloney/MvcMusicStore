using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddScoped<MusicStoreEntities>(_ =>
                {
                    var entities = new MusicStoreEntities(Configuration.GetConnectionString("MusicStoreEntities"));
                    entities.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
                    entities.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
                    return entities;
                }
                );
            services.AddTransient<SampleData>();
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
