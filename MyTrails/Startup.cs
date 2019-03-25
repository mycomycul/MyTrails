using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MyTrails.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyTrails.Startup))]
namespace MyTrails
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //Creating a default User and Admin role
            //Check if roles are already added if not create admin role
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }

            try //or??
            {
                var user = new ApplicationUser
                {

                    Email = "M@S.com",
                    PhoneNumber = "999-999-9999",
                    UserName = "Admin",

                };

                string userPassword = "Password1!";

                var chkUser = UserManager.Create(user, userPassword);

                //Adding default user to admin role
                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "Admin");
                }
            }
            catch (System.Exception)
            {

            }

            //Creating User Role
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "User"
                };
                roleManager.Create(role);
            }
            try //or??
            {
                var user = new ApplicationUser
                {
                    Email = "alexa@gmail.com",
                    PhoneNumber = "123-456-7894",
                    UserName = "User"
                };



                string userPassword = "Password1!";
                var chkUser = UserManager.Create(user, userPassword);

                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "User");
                }
            }
            catch (System.Exception)
            {

            }
        }
        }

}

