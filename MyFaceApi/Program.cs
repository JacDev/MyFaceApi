using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MyFaceApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();

			try
			{
				Log.Information("{time} - Application Starting Up", DateTime.Now);
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "The application faild to start correctly.");
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog()
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
