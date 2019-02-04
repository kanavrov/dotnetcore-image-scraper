using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JustInCase.ImageScraper
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var settings = new ScraperSettings
			{
				DriverPath = @"E:\TOOLS\Selenium",
				Url = "https://readcomiconline.to/Comic/Saga/Issue-37?id=86517&quality=hq&readType=1",
				ImageRootSelector = "#containerRoot #divImage",
				IncludeUrlPrefixes = new [] { "https://2.bp.blogspot.com/" },
				ImageSelector = "img",
				InitialWaitTimeMillis = 10000,
				DestinationFolderPath = @"C:\Temp\image-scraper"
			};
			         
			var scraper = new ImageUrlScraper(settings);
			var downloader = new ImageDownloader(settings);
			var persister = new ImagePersister(settings);

			persister.PersistAll(downloader.DownloadAll(scraper.Scrape()));
        }
    }
}
