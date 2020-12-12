using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using MyFaceApi.Api.Application.FileManagerInterfaces;

namespace MyFaceApi.Api.Application.FileManager
{
	public class ImageManager : IImageManager
	{
		public string ImagePath { get; set; }
		private readonly ILogger<ImageManager> _logger;
		public ImageManager(IConfiguration config, 
			ILogger<ImageManager> logger)
		{
			ImagePath = config["Path:Images"];
			Console.Write(ImagePath);
			_logger = logger;
			_logger.LogError(ImagePath);
		}

		public FileStream ImageStream(string imageName)
		{
			return new FileStream(Path.Combine(ImagePath, imageName), FileMode.Open, FileAccess.Read);
		}

		public async Task<Tuple<string, string>> SaveImage(IFormFile image)
		{
			try
			{
				var savePath = Path.Combine(ImagePath);
				if (!Directory.Exists(savePath))
				{
					Directory.CreateDirectory(savePath);
				}
				var mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
				var fileName = $"{DateTime.Now:dd-MM-yyy-HH-mm-ss}_{Guid.NewGuid()}{mime}";

				var fileStream = new FileStream(Path.Combine(savePath, fileName), FileMode.Create);

				await image.CopyToAsync(fileStream);
				fileStream.Close();

				return new Tuple<string, string>(fileName, Path.Combine(savePath, fileName));
			}
			catch
			{
				throw;
			}
		}
		public string AddProfileImage(string imageName, int width =100, int height=100)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			Image imgPhoto = Image.FromFile(Path.Combine(ImagePath, imageName));

			destImage.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.Clamp);
					graphics.DrawImage(imgPhoto, destRect, 0, 0, imgPhoto.Width, imgPhoto.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}
			var newPath = "p" + imageName;
			var savePath = Path.Combine(ImagePath);
			destImage.Save(Path.Combine(savePath, newPath));
			return newPath;
		}
	}
}
