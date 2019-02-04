namespace JustInCase.ImageScraper
{
	public class ScraperSettings
	{
		public string DriverPath { get; set; }

		public string Url { get; set; }

		public string ImageRootSelector { get; set; }

		public string ImageSelector { get; set; }

		public int InitialWaitTimeMillis { get; set; }

		public string[] IncludeUrlPrefixes { get; set; }

		public string[] ExcludeUrlPrefixes { get; set; }

		public string DestinationFolderPath { get; set; }

		public bool IsInfiniteScrollPage { get; set; }

		public int InfiniteScrollWaitTimeMillis { get; set; }

		public int InfiniteScrollStepPixels { get; set; }
	}
}