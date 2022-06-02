using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Riode.WebUI.AppCode.Providers;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riode.WebUI
{
    public class Startup
    {
        readonly IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(cfg => 
            {
                cfg.ModelBinderProviders.Insert(0, new BooleanBinderProvider());

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                cfg.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddRouting(cfg =>
            {
                cfg.LowercaseUrls = true;
            });
            
            services.AddDbContext<RiodeDbContext>(cfg =>
            {
                cfg.UseSqlServer(configuration.GetConnectionString("cString"));
            })  .AddIdentity<RiodeUser, RiodeRole>()
                .AddEntityFrameworkStores<RiodeDbContext>()
                .AddDefaultTokenProviders();
            
            services.Configure<IdentityOptions>(cfg =>
            {
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredUniqueChars = 1;
                cfg.Password.RequiredLength = 3;

                cfg.User.RequireUniqueEmail = true;
                //cfg.User.AllowedUserNameCharacters = "abcdef...";
                
                cfg.Lockout.MaxFailedAccessAttempts = 3;
                cfg.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0,3,0);
            });

            services.ConfigureApplicationCookie(cfg =>
            {
                cfg.LoginPath = "/signin.html";
                cfg.AccessDeniedPath = "/accessdenied.html";
                cfg.ExpireTimeSpan = new TimeSpan(0,5,0);
                cfg.Cookie.Name = "riode";
            });

            services.AddAuthentication();
            services.AddAuthorization(cfg =>
            {
                foreach (var policyName in Program.principals)
                {
                    cfg.AddPolicy(policyName, p =>
                    {
                        p.RequireAssertion(handler =>
                        {
                            return handler.User.IsInRole("Superadmin") || handler.User.HasClaim(policyName, "1");
                        });
                    });
                }
            });

            services.AddScoped<UserManager<RiodeUser>>();
            services.AddScoped<SignInManager<RiodeUser>>();
            services.AddScoped<RoleManager<RiodeRole>>();

            services.AddMediatR(this.GetType().Assembly);
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IClaimsTransformation, AppClaimProvider>();
            
            services.AddFluentValidation(cfg =>
            {
                cfg.RegisterValidatorsFromAssemblies(new[] { this.GetType().Assembly });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.InitDb();
            app.SeedMembership();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(cfg =>
            {
                //cfg.MapControllerRoute(
                //    name: "default-signin",
                //    pattern: "signin.html",
                //    defaults: new
                //    {
                //        area = "",
                //        controller = "account",
                //        action = "signin"
                //    });
                //cfg.MapControllerRoute(
                //    name: "default-register",
                //    pattern: "register.html",
                //    defaults: new
                //    {
                //        area = "",
                //        controller = "account",
                //        action = "register"
                //    });
                //cfg.MapControllerRoute(
                //    name: "default-register-confirm",
                //    pattern: "registration-confirm.html",
                //    defaults: new
                //    {
                //        area = "",
                //        controller = "account",
                //        action = "registerConfirm"
                //    });
                //cfg.MapControllerRoute(
                //    name: "default-logout",
                //    pattern: "logout.html",
                //    defaults: new
                //    {
                //        area = "",
                //        controller = "account",
                //        action = "logout"
                //    });
                cfg.MapControllerRoute(
                    name: "default-accessdenied",
                    pattern: "accessdenied.html",
                    defaults: new
                    {
                        area = "",
                        controller = "account",
                        action = "accessdenied"
                    });
                cfg.MapAreaControllerRoute(
                    name: "defaultAdmin",
                    areaName: "Admin",
                    pattern:"Admin/{controller=dashboard}/{action=index}/{id?}");
                cfg.MapControllerRoute(
                    name:"default", 
                    pattern: "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
