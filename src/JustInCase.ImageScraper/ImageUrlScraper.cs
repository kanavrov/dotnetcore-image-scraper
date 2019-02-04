using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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
				using (var driver = new ChromeDriver(_settings.DriverPath))
				{
					driver.Navigate().GoToUrl(_settings.Url);
					driver.Manage().Window.Maximize();

					var imageRoot = WaitForImageRoot(driver);

					if (_settings.IsInfiniteScrollPage)
					{
						HandleInfiniteScroll(driver);
					}

					var images = GetImages(imageRoot);
					var hostUrl = UrlHelper.GetHostUrl(_settings.Url);

					foreach (var image in images)
					{
						string url = GetImageUrl(image, hostUrl);
						if (!string.IsNullOrEmpty(url))
							results.Add(url);
					}
				}
			}
			catch (Exception e)
			{
				//TODO Log exception.
				throw;
			}

			return results;
		}

		private IWebElement WaitForImageRoot(RemoteWebDriver driver)
		{
			var waitTimeMillis = _settings.InitialWaitTimeMillis < 1 ? 1 : _settings.InitialWaitTimeMillis;
			Thread.Sleep(waitTimeMillis);

			var rootSelector = string.IsNullOrEmpty(_settings.ImageRootSelector) ? DefaultImageRootSelector : _settings.ImageRootSelector;

			return driver.FindElementByCssSelector(rootSelector);
		}

		private void HandleInfiniteScroll(RemoteWebDriver driver)
		{
			var waitTimeMillis = _settings.InfiniteScrollWaitTimeMillis < 1 ? 1 : _settings.InfiniteScrollWaitTimeMillis;
			var scrollScript = $"window.scrollBy(0, {_settings.InfiniteScrollStepPixels});";
			var validationScript = "return (window.scrollY === 0) || (window.innerHeight + window.scrollY) >= document.body.offsetHeight;";

			while(true)
			{
				driver.ExecuteScript(scrollScript);
				Thread.Sleep(waitTimeMillis);
				if((bool)driver.ExecuteScript(validationScript))
					break;
			}			
		}

		private List<IWebElement> GetImages(IWebElement imageRoot)
		{
			var imageSelector = string.IsNullOrEmpty(_settings.ImageSelector) ? DefaultImageSelector : _settings.ImageSelector;
			return new List<IWebElement>(imageRoot.FindElements(By.CssSelector(imageSelector)));
		}

		private string GetImageUrl(IWebElement image, string hostUrl)
		{
			string url = image.GetAttribute("src");

			if (string.IsNullOrEmpty(url))
				return url;

			var isIncluded = IsUrlIncluded(url, out int includedMatchLength);
			var isExluded = IsUrlExcluded(url, out int exludedMatchLength);

			if(!UrlHelper.IsAbsoluteUrl(url))
				url = UrlHelper.ToAbsoluteUrl(url, hostUrl);			

			return !isIncluded || (isExluded && exludedMatchLength > includedMatchLength) ? null : url;
		}

		private bool IsUrlIncluded(string url, out int matchLength)
		{
			matchLength = int.MinValue;

			if (_settings.IncludeUrlPrefixes == null || _settings.IncludeUrlPrefixes.Any())
				return true;

			foreach (var prefix in _settings.IncludeUrlPrefixes)
			{
				if (url.StartsWith(prefix, true, CultureInfo.InvariantCulture))
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

			if (_settings.ExcludeUrlPrefixes == null || _settings.ExcludeUrlPrefixes.Any())
				return false;

			foreach (var prefix in _settings.ExcludeUrlPrefixes)
			{
				if (url.StartsWith(prefix, true, CultureInfo.InvariantCulture))
				{
					matchLength = prefix.Length;
					return true;
				}
			}

			return false;
		}
	}
}