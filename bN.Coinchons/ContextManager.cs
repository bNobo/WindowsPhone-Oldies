using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.IO;
using bN.Core;
using System.Diagnostics;
using bN.Coinchons;
using System.Runtime.Serialization.Json;

namespace bN.Coinchons
{
	public class ContextManager
	{
		private const string _fileName = "context.json";

		public static async System.Threading.Tasks.Task SaveContext()
		{
			var currentFrame = Window.Current.Content as Frame;

			if (currentFrame.Content is MainPage)
			{
				var viewModel = (currentFrame.Content as MainPage).DataContext;
				var memoryStream = new MemoryStream();
				var serializer = new DataContractJsonSerializer(typeof(MainViewModel));
				serializer.WriteObject(memoryStream, viewModel);

				var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
					_fileName, CreationCollisionOption.ReplaceExisting);

				using (Stream fileStream = await file.OpenStreamForWriteAsync())
				{
					memoryStream.Seek(0, SeekOrigin.Begin);
					await memoryStream.CopyToAsync(fileStream);
				}

				//var memoryStream = new MemoryStream();

				//var dico = new Dictionary<string, object>();
				//dico.Add("abc", "def");
				//dico.Add("ghi", 0);
				//var serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
				//serializer.WriteObject(memoryStream, dico);
				//TestViewModel test = new TestViewModel { Valeur = 1 };
				//var serializer = new DataContractJsonSerializer(typeof(TestViewModel));
				//try
				//{
				//	serializer.WriteObject(memoryStream, test);
				//	memoryStream.Seek(0, SeekOrigin.Begin);
				//	var streamReader = new StreamReader(memoryStream);
				//	Debug.WriteLine(streamReader.ReadToEnd());
				//}
				//catch (Exception ex)
				//{
				//	Debug.WriteLine(ex);
				//	return;
				//}
			}
		}

		public static async Task RestoreContext()
		{
			var currentFrame = Window.Current.Content as Frame;

			if (currentFrame.Content is MainPage)
			{
				await RestoreContext(currentFrame);
			}
		}

		internal static async Task RestoreContext(Frame currentFrame)
		{
			try
			{
				var file = await ApplicationData.Current.TemporaryFolder
										   .GetFileAsync(_fileName);

				using (var stream = await file.OpenStreamForReadAsync())
				{
					var vm = (MainViewModel)new DataContractJsonSerializer(
							typeof(MainViewModel)).ReadObject(stream);

					vm.RegisterScoreItemPropertyChanged();

					(currentFrame.Content as MainPage).DataContext = vm;
				}
			}
			catch
			{
				// En cas d'erreur bon bin tant pis, mieux vaut un contexte perdu qu'une
				// appli qui plante à ce stade.
			}
		}
	}
}
