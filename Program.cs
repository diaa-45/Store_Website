using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STORE_Website.Data;
using STORE_Website.Models;
using STORE_Website.Services;
using System;

namespace STORE_Website
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped(typeof(IReposirory<>), typeof(Repository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var connectionString = builder.Configuration.GetConnectionString("cs")
                ?? throw new Exception("Connection string 'cs' not found."); ;
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
            );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredLength = 4;
                option.Password.RequireDigit = false;
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters = "";
            }).AddEntityFrameworkStores<ApplicationDbContext>();



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                if (!await roleManger.RoleExistsAsync("Manager"))
                {
                    await roleManger.CreateAsync(new IdentityRole("Manager"));
                    await roleManger.CreateAsync(new IdentityRole("Admin"));
                    await roleManger.CreateAsync(new IdentityRole("User"));
                }
                
                var admin = await userManger.FindByEmailAsync("admin@gmail.com");
                if (admin==null)
                {
                   
                    admin = new ApplicationUser
                    {
                        UserName= "admin",
                        Email = "admin@gmail.com",
                        FullName = "admin",
                        City = "Al a`rish",
                        Address = "Al a`rish"
                    };
                    
                    IdentityResult result = await userManger.CreateAsync(admin, "admin");
                    if (result.Succeeded)
                    {
                        await userManger.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create admin user.");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                var manager = await userManger.FindByEmailAsync("manager@gmail.com");
               
                if (manager == null)
                {
                    manager = new ApplicationUser
                    {
                        UserName= "manager",
                        Email = "manager@gmail.com",
                        FullName = "manager",
                        City = "Al a`rish",
                        Address = "Al a`rish"
                    };
                    
                    IdentityResult result = await userManger.CreateAsync(manager, "manager");
                    if (result.Succeeded)
                    {
                        await userManger.AddToRoleAsync(manager, "Manager");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create admin user.");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
