using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace JustInCase.ImageScraper
{
    public class ImageUrlScraper
    {
		private const string DefaultImageRootSelector = "body";

		private const string DefaultImageSelector = "img";

		private readonly ScraperSettings _settings;

		public ImageUrlScraper(ScraperSettings settings)
		{
			_settings = settings;
		}

		public List<string> Scrape()
		{
			var results = new List<string>();

			try
			{
				using(var driver = new ChromeDriver(_settings.DriverPath))
				{
					driver.Navigate().GoToUrl(_settings.Url);
					var imageRoot = WaitForImageRoot(driver);
					var images = GetImages(imageRoot);

					foreach(var image in images)
					{
						string url = GetImageUrl(image);
						if(!string.IsNullOrEmpty(url))
							results.Add(url);
					}
				}
			}
			catch(Exception e)
			{
				//TODO Log exception.
				throw;
			}

			return results;
		}

		private IWebElement WaitForImageRoot(RemoteWebDriver driver)
		{
			var waitTimeSeconds = _settings.InitialWaitTimeSeconds < 1 ? 1 : _settings.InitialWaitTimeSeconds;
			Thread.Sleep(waitTimeSeconds * 1000);
			
			var rootSelector = string.IsNullOrEmpty(_settings.ImageRootSelector) ? DefaultImageRootSelector : _settings.ImageRootSelector;

			return driver.FindElementByCssSelector(rootSelector);
		}

		private List<IWebElement> GetImages(IWebElement imageRoot)
		{
			var imageSelector = string.IsNullOrEmpty(_settings.ImageSelector) ? DefaultImageSelector : _settings.ImageSelector;
			return new List<IWebElement>(imageRoot.FindElements(By.CssSelector(imageSelector)));
		}

		private string GetImageUrl(IWebElement image)
		{
			string url = image.GetAttribute("src");

			if(string.IsNullOrEmpty(url))
				return url;

			var isIncluded = IsUrlIncluded(url, out int includedMatchLength);
			var isExluded = IsUrlExcluded(url, out int exludedMatchLength);
			
			return !isIncluded || (isExluded && exludedMatchLength > includedMatchLength) ? null : url;
		}

		private bool IsUrlIncluded(string url, out int matchLength)
		{
			matchLength = int.MinValue;

			if(_settings.IncludeUrlPrefixes == null || _settings.IncludeUrlPrefixes.Any())
				return true;
			
			foreach(var prefix in _settings.IncludeUrlPrefixes)
			{
				if(url.StartsWith(prefix, true, CultureInfo.InvariantCulture))
				{
					matchLength = prefix.Length;
					return true;
				}
					
			}

			return false;
		}

		private bool IsUrlExcluded(string url, out int matchLength)
		{
			matchLength = int.MinValue;

			if(_settings.ExcludeUrlPrefixes == null || _settings.ExcludeUrlPrefixes.Any())
				return false;
			
			foreach(var prefix in _settings.ExcludeUrlPrefixes)
			{
				if(url.StartsWith(prefix, true, CultureInfo.InvariantCulture))
				{
					matchLength = prefix.Length;
					return true;
				}
			}

			return false;
		}
    }
}