using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Spire.Doc;
using Spire.Doc.Documents;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using Newtonsoft.Json.Linq;

namespace Converters {
	public static class Converter {
		private static string tempPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
		private static string subscriptionKey = "9243aac33d7f41f4a0dbe1430bdabd05";
		private static string endpoint = "https://hyperrocog.cognitiveservices.azure.com/";

		public static string PdfToStringConvert(string path) {
			var res = new StringBuilder();
			using (PdfDocument document = PdfDocument.Open(path)) {
				foreach (UglyToad.PdfPig.Content.Page page in document.GetPages()) {
					string pageText = page.Text;

					foreach (UglyToad.PdfPig.Content.Word word in page.GetWords()) {
						res.Append(word + " ");
					}
				}
			}

			return res.ToString();
		}
		public static string TxtToStringConvert(string path) {
			return File.ReadAllText(path);
		}
		public static string DocToStringConverter(string path) {
			//Load Document
			Document document = new Document();
			document.LoadFromFile(path);

			//Initialzie StringBuilder Instance
			StringBuilder sb = new StringBuilder();

			//Extract Text from Word and Save to StringBuilder Instance
			foreach (Section section in document.Sections) {
				foreach (Paragraph paragraph in section.Paragraphs) {
					sb.AppendLine(paragraph.Text);
				}
			}

			return sb.ToString();
		}

		public static string JpegToStringConverter(string path) {
			var res = RecognizePrintedTextLocal(path);

			res.Wait();
			return res.Result;
		}

		public static async Task<string> RecognizePrintedTextLocal(string localImage) {
			string res = "";

			// Создадим запрос.
			var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10)};
			var queryString = HttpUtility.ParseQueryString(string.Empty);

			// Вводим ключ.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9243aac33d7f41f4a0dbe1430bdabd05");

			// Параметры запроса.
			queryString["language"] = "en";
			queryString["detectOrientation"] = "true";
			var uri = "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/ocr?" + queryString;

			HttpResponseMessage httpResponseMessage;

			// Изображение для запроса.
			byte[] byteData = File.ReadAllBytes(localImage);

			// Делаем запрос.
			using (var content = new ByteArrayContent(byteData)) {
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				httpResponseMessage = await client.PostAsync(uri, content);
			}
			var responseString = await httpResponseMessage.Content.ReadAsStringAsync();

			// Проверяем успешность запроса.
			if (httpResponseMessage.IsSuccessStatusCode) {
				// Перевод JSON формата.
				JObject o = JObject.Parse(responseString);
				var qe = from region in o["regions"]
						 from line in region["lines"]
						 from word in line["words"]
						 select (string)word["text"];

				res = String.Join(' ', qe.ToArray());
			}
			else Toast.MakeText(null, "Неудачный запрос", ToastLength.Short).Show();
			client.Dispose();

			return res;
		}

		private static string GetFileName(string path) {
			var res = Regex.Match(path, @"([^.\\]+?)(?=\.)");
			if (!res.Success)
				throw new ArgumentException($"Невозможно извлечь имя файла из пути {path}");

			return res.Value;
		}

		public static ComputerVisionClient Authenticate(string endpoint, string key) {
			ComputerVisionClient client =
			  new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
			return client;
		}
	}
}