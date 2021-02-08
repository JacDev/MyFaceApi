using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Logging;

namespace MyFaceApi.IdentityServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();


			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
						.ConfigureLogging(logging => logging.AddAzureWebAppDiagnostics())
			.ConfigureServices(serviceCollection => serviceCollection
				.Configure<AzureFileLoggerOptions>(options =>
				{
					options.FileName = "azure-diagnostics-";
					options.FileSizeLimit = 50 * 1024;
					options.RetainedFileCountLimit = 5;
				}).Configure<AzureBlobLoggerOptions>(options =>
				{
					options.BlobName = "log.txt";
				})
)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}

}
