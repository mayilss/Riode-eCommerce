using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riode.WebUI.Models.Entities;
using Riode.WebUI.Models.Entities.Membership;
using System;
using System.Linq;

namespace Riode.WebUI.Models.DataContexts
{
    public static class RiodeDbSeed
    {
        static internal IApplicationBuilder InitDb(this IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.CreateScope()){
                var db = scope.ServiceProvider.GetRequiredService<RiodeDbContext>();
                db.Database.Migrate();
                InitPostTags(db);
            }
            return app;
        }

        private static void InitPostTags(RiodeDbContext db)
        {
            if (!db.PostTags.Any())
            {
                db.PostTags.AddRange(new[]
                {
                    new PostTag{Name="Bag"},
                    new PostTag{Name="Classic"},
                    new PostTag{Name="Converse"},
                    new PostTag{Name="Leather"},
                    new PostTag{Name="Fit"},
                    new PostTag{Name="Grren"},
                    new PostTag{Name="Man"},
                    new PostTag{Name="Woman"},
                    new PostTag{Name="Jeans"}
                });
                db.SaveChanges();
            }
        }

        public static IApplicationBuilder SeedMembership(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var role = new RiodeRole
                {
                    Name = "Superadmin"
                };
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RiodeUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RiodeRole>>();
                if (roleManager.RoleExistsAsync(role.Name).Result)
                {
                    role = roleManager.FindByNameAsync(role.Name).Result;
                }
                else
                {
                    var roleCreateResult = roleManager.CreateAsync(role).Result;
                    if (!roleCreateResult.Succeeded)
                    {
                        goto end;
                    }
                }
                string pwd = "123";
                var user = new RiodeUser
                {
                    UserName = "mayilss",
                    Name = "Mayil",
                    Surname = "Safarov",
                    Email = "mayilsafarov@gmail.com",
                    EmailConfirmed = true,
                };
                var foundUser = userManager.FindByEmailAsync(user.Email).Result;
                if (foundUser != null && !userManager.IsInRoleAsync(foundUser, role.Name).Result)
                {
                    userManager.AddToRoleAsync(foundUser, role.Name).Wait();
                }
                else if (foundUser == null)
                {
                    var userCreateResult = userManager.CreateAsync(user, pwd).Result;
                    if (!userCreateResult.Succeeded)
                    {
                        goto end;
                    }
                    userManager.AddToRoleAsync(user, role.Name).Wait();
                }
            }
            end:
            return app;
        }
    }
}
