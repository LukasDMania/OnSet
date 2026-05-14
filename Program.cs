
using FluentValidation;
using MediatR;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnSet.Domain;
using OnSet.Domain.Models;
using OnSet.Extensions;
using OnSet.Infrastructure.Behaviors;
using OnSet.Infrastructure.Data;
using OnSet.Infrastructure.Filters;
using OnSet.Infrastructure.Persistence;
using OnSet.Infrastructure.Services;
using OnSet.Utils;

namespace OnSet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            // --- 1. EF Core Setup ---
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddScoped<AuditingSaveChangesInterceptor>();
            builder.Services.AddDbContext<OnSetDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>());
            });

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<OnSetDbContext>("database");

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Production: send confirmation email and leave EmailConfirmed false until the user confirms.
                // Today registration sets EmailConfirmed true (see Register CommandHandler), so this is a dev-friendly shortcut.
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<OnSetDbContext>()
            .AddDefaultTokenProviders();


            // --- 2. MediatR Setup ---
            // Register MediatR, scanning the entire assembly for handlers, validators, and behaviors
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            //Add automapper
            builder.Services.AddAutoMapper(typeof(Program));

            // --- 3. Validation & Pipeline Behaviors ---
            // Register all IValidator implementations from the assembly
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            builder.Services.Configure<PerformanceTelemetryOptions>(
                builder.Configuration.GetSection(PerformanceTelemetryOptions.SectionName));

            // Pipeline order (outer → inner): command audit → performance (validation+handler) → validation → handler.
            // Performance sits inside audit so timings exclude command-audit DB I/O.
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandAuditBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Add Services
            builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ICommandAuditService, CommandAuditService>();
            builder.Services.AddScoped<IStorageService, LocalStorageService>();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
            });

            // Add services to the container.
            builder.Services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add<DomainExceptionFilter>();
                });

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.ApplyMigrations();

                using var scope = app.Services.CreateScope();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(Roles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                }
                if (!await roleManager.RoleExistsAsync(Roles.StandardUser))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.StandardUser));
                }
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           

            app.MapHealthChecks("/health");
            app.MapRazorPages();

            try
            {
                await app.RunAsync();
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
