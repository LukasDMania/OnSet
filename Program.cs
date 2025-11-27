
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain;
using OnSet.Domain.Models;
using OnSet.Extensions;
using OnSet.Infrastructure.Behaviors;
using OnSet.Infrastructure.Data;

namespace OnSet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. EF Core Setup ---
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            //Db setup
            builder.Services.AddDbContext<OnSetDbContext>(options =>
                options.UseSqlServer(connectionString));

        

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<OnSetDbContext>()
                .AddApiEndpoints();


            // --- 2. MediatR Setup ---
            // Register MediatR, scanning the entire assembly for handlers, validators, and behaviors
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            //Add automapper
            builder.Services.AddAutoMapper(typeof(Program));

            // --- 3. Validation & Pipeline Behaviors ---
            // Register all IValidator implementations from the assembly
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            //Test

            // Register the Validation Behavior in the MediatR Pipeline
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            else
            {
                app.ApplyMigrations();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           

            app.MapRazorPages();

            app.MapIdentityApi<User>();

            app.Run();
        }
    }
}
