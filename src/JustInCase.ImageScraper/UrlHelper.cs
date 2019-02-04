using System;

namespace JustInCase.ImageScraper
{
	public static class UrlHelper
	{
		public static bool IsAbsoluteUrl(string url)
		{
    		Uri result;
    		return Uri.TryCreate(url, UriKind.Absolute, out result);
		}

		public static string ToAbsoluteUrl(string relativeUrl, string hostUrl)
		{
			return new Uri(new Uri(hostUrl), relativeUrl).AbsoluteUri;
		}

		public static string GetHostUrl(string absoluteUrl)
		{
			return new Uri(absoluteUrl).GetLeftPart(UriPartial.Authority);
		}
	}
}
