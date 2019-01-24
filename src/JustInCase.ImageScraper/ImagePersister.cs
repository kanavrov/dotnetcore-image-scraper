using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace JustInCase.ImageScraper
{
	public class ImagePersister
	{
		private readonly ScraperSettings _settings;

		private readonly FileExtensionResolver _fileExtensionResolver;

		public ImagePersister(ScraperSettings settings)
		{
			_settings = settings;
			_fileExtensionResolver = new FileExtensionResolver();
		}

		public void PersistAll(List<ImageContent> images)
		{
			var batchId = Guid.NewGuid();
			PersistZip(batchId, images, ".cbr");
		}

		private void PersistZip(Guid batchId, List<ImageContent> images, string extension)
		{
			string destinationPath = Path.Combine(_settings.DestinationFolderPath, $"{batchId}{extension}");

			using (var compressedFileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
				{
					for (int i = 0; i < images.Count; i++)
					{
						var image = images[i];
						var fileName = BuildFileName(image, i, images.Count, batchId);
						var zipEntry = zipArchive.CreateEntry(fileName);

						using (var zipEntryStream = zipEntry.Open())
						{
							foreach (var b in image.Content)
							{
								zipEntryStream.WriteByte(b);
							}
						}
					}
				}
			}
		}

		private string BuildFileName(ImageContent image, int index, int totalCount, Guid batchId)
		{
			var padding = (totalCount - totalCount % 10) / 10 + 1;
			var extension = _fileExtensionResolver.Resolve(image.MimeType);
			return $"{index.ToString().PadLeft(padding, '0')}_{batchId}{extension}";
		}
	}
}