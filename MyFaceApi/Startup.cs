using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFaceApi.Api.Application;
using MyFaceApi.Api.Hubs;
using MyFaceApi.Api.Infrastructure;
using MyFaceApi.Api.Infrastructure.Database;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
			string clientUri = Configuration.GetValue<string>("ClientUri");
			services.AddCors(options =>
			{
				// this defines a CORS policy called "default"
				options.AddPolicy("default", policy =>
				{
					policy.WithOrigins(clientUri)
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
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
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["token"];

						var path = context.HttpContext.Request.Path;
						if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notificationHub")))
						{
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
				};
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
					options.UseSqlServer(Configuration.GetConnectionString("MyFaceApi")
						,b => b.MigrationsAssembly("MyFaceApi.Api"));
					});
			services.AddDbContext<OnlineUserDbContext>(
				options => options.UseInMemoryDatabase("OnlineUsers")
				);
			services.AddHttpClient();
			services.AddInfrastructure();
			services.AddApplication();
			services.AddSignalR();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("v1/swagger.json", "My API V1");
					c.DocumentTitle = "Api Doc";
				});
			}
			app.UseHsts();
			app.UseHttpsRedirection();

			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseCors("default");

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<NotificationHub>("/notificationHub");
			});
		}
	}
}
