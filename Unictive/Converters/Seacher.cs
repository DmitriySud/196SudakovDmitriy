using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace Converters {
	class Seacher {
		public enum FileExtensions : byte {
			txt = 0b0001,
			pdf = 0b0010,
			word = 0b0100,
			img = 0b1000,
		}
		private static Dictionary<FileExtensions, Func<string, string>> converters
			= new Dictionary<FileExtensions, Func<string, string>> {
				{ FileExtensions.txt, new Func<string, string>(Converters.Converter.TxtToStringConvert)},
				{ FileExtensions.pdf, new Func<string, string>(Converters.Converter.PdfToStringConvert)},
				{ FileExtensions.word, new Func<string, string>(Converters.Converter.DocToStringConverter)},
				{ FileExtensions.img, new Func<string, string>(Converters.Converter.JpegToStringConverter)},
			};

		private static Dictionary<string, FileExtensions> stringToExtention =
			new Dictionary<string, FileExtensions> {
				{ ".txt", FileExtensions.txt},
				{ ".pdf", FileExtensions.pdf},
				{ ".doc", FileExtensions.word},
				{ ".docx", FileExtensions.word},
				{ ".jpeg", FileExtensions.img},
				{ ".jpg", FileExtensions.img},
			};

		public class SearchResult {
			public SearchResult(string fileName, string match) {
				FileName = fileName;
				Match = match;
			}

			/// <summary> Имя файла. </summary>
			public string FileName { get; set; }
			/// <summary> Строка с предыдущими и последующими 10-ю символами. </summary>
			public string Match { get; set; }
		}


		public static string SearchInText(string text, string word) {
			int idx = text.IndexOf(word,StringComparison.OrdinalIgnoreCase);
			if (idx == -1)
				return null;

			int left = Math.Max(idx - 10, 0);
			return (left == 0 ? "" : "...") + text.Substring(left, 20 + word.Length) + "...";
		}

		public static SearchResult SearchInFile(string path, FileExtensions ext, string word) {
			var res = new SearchResult(path, SearchInText(converters[ext](path), word));
			return res.Match is null ? null : res;
		}

		public static List<SearchResult> SearchInFolder(string path, FileExtensions ext, string word) {
			var res = new List<SearchResult>();

			var filesPaths = Directory.GetFiles(path);
			var dirPaths = Directory.GetDirectories(path);

			foreach (string filePath in filesPaths) {
				var curExt = GetExtension(filePath);
				if ((byte)(curExt & ext) != 0) {
					var match = SearchInFile(filePath, curExt, word);
					if (!(match is null))
						res.Add(match);
				}
			}

			foreach (string dirPath in dirPaths) {
				res.AddRange(SearchInFolder(dirPath, ext, word));
			}

			return res;
		}

		private static FileExtensions GetExtension(string path) {
			var res = Regex.Match(path, @"\.\w+$");
			if (!res.Success)
				throw new ArgumentException("Невозможно извлечь расширение.");
			if (!stringToExtention.ContainsKey(res.Value))
				return 0;
			return stringToExtention[res.Value];
		}
	}
}