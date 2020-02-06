using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vibe.Components;
using Vibe.Data;
using Vibe.Models;
using Vibe.Services;

public class Startup
{
	public IConfiguration Configuration
	{
		get;
	}

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddDbContext<ApplicationDbContext>(delegate (DbContextOptionsBuilder options)
		{
			options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
		});
		services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
		services.Configure(delegate (IdentityOptions options)
		{
			options.Password.RequireDigit = false;
			options.Password.RequiredLength = 8;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireLowercase = false;
			options.Password.RequiredUniqueChars = 6;
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30.0);
			options.Lockout.MaxFailedAccessAttempts = 10;
			options.Lockout.AllowedForNewUsers = true;
			options.User.RequireUniqueEmail = true;
		});
		services.ConfigureApplicationCookie(delegate (CookieAuthenticationOptions options)
		{
			options.Cookie.HttpOnly = true;
			options.ExpireTimeSpan = TimeSpan.FromMinutes(10.0);
			options.LoginPath = "/Account/Login";
			options.LogoutPath = "/Account/Logout";
			options.AccessDeniedPath = "/Account/AccessDenied";
			options.SlidingExpiration = true;
		});
		services.AddTransient<IEmailSender, EmailSender>();
		services.AddMvc();
		services.AddScoped<IDbInitializer, DbInitializer>();
		services.AddScoped<PageLookupService>();
		services.Configure<AuthMessageSenderOptions>(Configuration);
		services.AddMvc(o => o.EnableEndpointRouting = false);
	}

	public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDbInitializer dbInitializer)
	{
		if (env.IsDevelopment())
		{
			app.UseBrowserLink();
			app.UseDeveloperExceptionPage();
			app.UseDatabaseErrorPage();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
		}
		app.UseStaticFiles();
		app.UseAuthentication();
		dbInitializer.Initialize();
		app.UseMiddleware<UrlRewritingMiddleware>(Array.Empty<object>());
		app.UseMvc(delegate (IRouteBuilder routes)
		{
			routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
		});
	}
}
