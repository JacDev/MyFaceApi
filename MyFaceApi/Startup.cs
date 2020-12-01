using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFaceApi.Api.DataAccess.Data;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.IdentityServerAccess;
using MyFaceApi.Api.Repository;
using MyFaceApi.Api.Repository.Interfaces;
using MyFaceApi.Api.Servieces;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.IO;
using System.Reflection;

namespace MyFaceApi
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
			services.AddCors(options =>
			{
				// this defines a CORS policy called "default"
				options.AddPolicy("default", policy =>
				{
					policy.WithOrigins("http://localhost:4200")
						.AllowAnyHeader()
						.AllowAnyMethod();
						//.AllowCredentials();
					//.AllowAnyOrigin();
				});
			});


			IConfigurationSection identityServerConf = Configuration.GetSection("IdentityServerConfiguration");
			var identityServerUrl = identityServerConf.GetValue<string>("IdentityServerUri");
			var audienceName = identityServerConf.GetValue<string>("AudienceName");

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.Authority = identityServerUrl;
				options.Audience = audienceName;
			});

			services.AddControllers(options =>
			{
				options.ReturnHttpNotAcceptable = true;
				var policy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();

				options.Filters.Add(new AuthorizeFilter(policy));
			})
				.AddNewtonsoftJson(setupAction =>
				{
					setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				});

			services.AddSwaggerGen(c =>
			{
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			services.AddDbContext<AppDbContext>(
				options => {
					options.UseSqlServer(Configuration.GetConnectionString("MyFaceApi"),
					b => b.MigrationsAssembly("MyFaceApi.Api"));
					});


			services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
		
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			services.AddRepositories();

			services.AddHttpClient();
			services.AddScoped<IIdentityServerHttpService, IdentityServerHttpService>();
			
			
			services.AddScoped<IUserRepository, UserIdentityServerAccess>();
			
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.DocumentTitle = "Api Doc";
			});



			app.UseHttpsRedirection();

			//przy ka�dym zapytaniu http zapisuje logi
			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseCors("default");

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
