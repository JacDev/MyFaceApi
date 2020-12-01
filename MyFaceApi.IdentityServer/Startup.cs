using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
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
using IdentityServer4.EntityFramework.Mappers;

using System.Linq;

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
					options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("ConfigConnection"),
						  sql => sql.MigrationsAssembly(assembly));
				})
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("PersistedConnection"),
						  sql => sql.MigrationsAssembly(assembly));
				})
				.AddDeveloperSigningCredential(); //add rsa key

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			InitializeDatabase(app);
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
		private void InitializeDatabase(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

				var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
				context.Database.Migrate();
				if (!context.Clients.Any())
				{
					foreach (var client in AuthConfiguration.GetClients())
					{
						context.Clients.Add(client.ToEntity());
					}
					context.SaveChanges();
				}

				if (!context.IdentityResources.Any())
				{
					foreach (var resource in AuthConfiguration.GetIdentityResources())
					{
						context.IdentityResources.Add(resource.ToEntity());
					}
					context.SaveChanges();
				}

				if (!context.ApiResources.Any())
				{
					foreach (var resource in AuthConfiguration.GetApis())
					{
						context.ApiResources.Add(resource.ToEntity());
					}
					context.SaveChanges();
				}
				if (!context.ApiScopes.Any())
				{
					foreach (var resource in AuthConfiguration.GetApiScopes())
					{
						context.ApiScopes.Add(resource.ToEntity());
					}
					context.SaveChanges();
				}
			}
		}
	}
}
