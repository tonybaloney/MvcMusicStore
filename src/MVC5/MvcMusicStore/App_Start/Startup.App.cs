﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcMusicStore.Models;
using Owin;
using System.Configuration;
using System.Data.Entity;

namespace MvcMusicStore
{
    public partial class Startup
    {
        public void ConfigureApp(IAppBuilder app)
        {
            Database.SetInitializer(new SampleData());

            CreateAdminUser();
        }

        private async void CreateAdminUser()
        {
            string _username = ConfigurationManager.AppSettings["DefaultAdminUsername"];
            string _password = ConfigurationManager.AppSettings["DefaultAdminPassword"];
            string _role = "Administrator";

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());

            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role = new IdentityRole(_role);
            var result = await roleManager.RoleExistsAsync(_role);
            if (result == false)
            {
                await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByNameAsync(_username);
            if (user == null)
            {
                user = new ApplicationUser { UserName = _username };
                await userManager.CreateAsync(user, _password);
                await userManager.AddToRoleAsync(user.Id, _role);
            }
        }
    }
}