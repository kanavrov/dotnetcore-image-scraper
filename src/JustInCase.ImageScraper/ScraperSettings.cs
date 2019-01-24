namespace JustInCase.ImageScraper
{
	public class ScraperSettings
	{
		public string DriverPath { get; set; }

		public string Url { get; set; }

		public string ImageRootSelector { get; set; }

		public string ImageSelector { get; set; }

		public int InitialWaitTimeSeconds { get; set; }

		public string[] IncludeUrlPrefixes { get; set; }

		public string[] ExcludeUrlPrefixes { get; set; }

		public string DestinationFolderPath { get; set; }
	}
}