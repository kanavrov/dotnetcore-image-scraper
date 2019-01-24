using System.Collections.Generic;

namespace JustInCase.ImageScraper
{
	public class FileExtensionResolver
	{
		private static Dictionary<string, string> MimeTypeMapping = new Dictionary<string, string>
		{
			{ "image/bmp", ".bmp" },
			{ "image/cis-cod", ".cod" },
			{ "image/gif", ".gif" },
			{ "image/ief", ".ief" },
			{ "image/jpeg", ".jpeg" },
			{ "image/pipeg", ".jfif" },
			{ "image/svg+xml", ".svg" },
			{ "image/tiff", ".tiff" },
			{ "image/x-cmu-raster", ".ras" },
			{ "image/x-cmx", ".cmx" },
			{ "image/x-icon", ".ico" },
			{ "image/x-portable-anymap", ".pnm" },
			{ "image/x-portable-bitmap", ".pbm" },
			{ "image/x-portable-graymap", ".pgm" },
			{ "image/x-portable-pixmap", ".ppm" },
			{ "image/x-rgb", ".rgb" },
			{ "image/x-xbitmap", ".xbm" },
			{ "image/x-xpixmap", ".xpm" },
			{ "image/x-xwindowdump", ".xwd" },
			{ "image/png", ".png" }
		};

		public string Resolve(string mimeType){
			return MimeTypeMapping.ContainsKey(mimeType) ? MimeTypeMapping[mimeType] : string.Empty;
		}
	}
}