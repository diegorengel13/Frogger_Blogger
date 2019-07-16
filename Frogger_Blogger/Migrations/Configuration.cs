namespace Frogger_Blogger.Migrations
{
    using Frogger_Blogger.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Frogger_Blogger.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Frogger_Blogger.Models.ApplicationDbContext context)
        {


            #region rolemanager
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if(!context.Roles.Any(r => r.Name == "moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "moderator" });
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            #endregion
            //i need to create a few users that will eventually occupy the roles of either admin or moderator
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if(!context.Users.Any(u => u.Email == "Moderator@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Moderator@Mailinator.com",
                    Email = "Moderator@Mailinator.com",
                    FirstName = "Mode",
                    LastName = "Rator",
                    DisplayName = "Moderator",
                }, "Abc&123!");
                   
            }



            if (!context.Users.Any(u => u.Email == "user@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "user@Mailinator.com",
                    Email = "user@Mailinator.com",
                    FirstName = "Use",
                    LastName = "Ser",
                    DisplayName = "User",
                }, "Abc&123!");

            }
            var userId = userManager.FindByEmail("user@mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("Moderator@mailinator.com").Id;
            userManager.AddToRole(userId, "Moderator");
        }
    }
}
