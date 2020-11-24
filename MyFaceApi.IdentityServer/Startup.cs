using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFaceApi.IdentityServer.DataAccess.Data;
using MyFaceApi.IdentityServer.DataAccess.Entities;
using System;

namespace MyFaceApi.IdentityServer
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
			services.AddControllersWithViews();
			var connectionString = Configuration.GetConnectionString("DefaultConnection");

			services.AddDbContext<IdentityServerDbContext>(
				options => {
					options.UseSqlServer(connectionString,
					b => b.MigrationsAssembly("MyFaceApi.IdentityServer"));
				});

			services.AddIdentity<AppUser, IdentityRole<Guid>>(config =>
			{
				config.Password.RequiredLength = 1;
				config.Password.RequireDigit = false;
				config.Password.RequireNonAlphanumeric = false;
				config.Password.RequireUppercase = false;
				config.Password.RequiredUniqueChars = 0;
				config.Password.RequireLowercase = false;
				config.SignIn.RequireConfirmedEmail = false;

			})
				.AddEntityFrameworkStores<IdentityServerDbContext>()
				.AddDefaultTokenProviders();

			services.ConfigureApplicationCookie(config =>
			{
				config.Cookie.Name = "IdentityServer.Cookie";
				config.LoginPath = "/Auth/Login";
				config.LogoutPath = "/Auth/Logout";

			});

			var assembly = typeof(Startup).Assembly.GetName().Name;

			services.AddIdentityServer()
				.AddAspNetIdentity<AppUser>()
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
						  sql => sql.MigrationsAssembly(assembly));
				})
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
						  sql => sql.MigrationsAssembly(assembly));
				})
				.AddDeveloperSigningCredential(); //add rsa key

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseIdentityServer();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
