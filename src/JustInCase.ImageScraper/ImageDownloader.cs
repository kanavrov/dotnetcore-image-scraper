using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace JustInCase.ImageScraper
{
	public class ImageDownloader
	{
		private const string ContentTypeHeader = "Content-Type";
		private readonly ScraperSettings _settings;
		public ImageDownloader(ScraperSettings settings)
		{
			_settings = settings;			
		}

		public List<ImageContent> DownloadAll(List<string> imageUrls)
		{
			var results = new List<ImageContent>();

			foreach(var url in imageUrls)
			{
				results.Add(DownloadSingle(url));
			}

			return results;
		}

		public ImageContent DownloadSingle(string imageUrl)
		{
			try
			{
				using(var client = new WebClient())
				{
					using(var stream = client.OpenRead(imageUrl))
					{
						using(var memoryStream = new MemoryStream(1024))
						{
  							stream.CopyTo(memoryStream);
  							return new ImageContent 
							  {
								  Content = memoryStream.ToArray(),
								  MimeType = client.ResponseHeaders[ContentTypeHeader]
							  };
						}
					}
				}
			}
			catch(Exception e)
			{
				//TODO Log exception
				throw;
			}
		}
	}
}