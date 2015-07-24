using System;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using ReactiveUI;
using Refit;

using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Services.Media;

namespace Issues
{
	public class NewIssueViewModel : BaseViewModel
	{
		public ImageSource Source {
			get { return source; }
			set { this.RaiseAndSetIfChanged (ref source, value); }
		}

		public NewIssueViewModel ()
		{
			var device = Resolver.Resolve<IDevice> ();
			mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;
		}

		public async Task<MediaFile> TakePhoto ()
		{
			Source = null;

			var options = new CameraMediaStorageOptions { DefaultCamera = CameraDevice.Rear, MaxPixelDimension = 300 };
			return await mediaPicker.TakePhotoAsync (options).ContinueWith (t => {
				if (!t.IsFaulted && !t.IsCanceled) {
					var mediaFile = t.Result;

					Source = ImageSource.FromStream (() => mediaFile.Source);
					imageStream = mediaFile.Source;

					return mediaFile;
				}

				return null;
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		public async Task SelectPhoto ()
		{
			Source = null;

			try {
				var options = new CameraMediaStorageOptions {
					DefaultCamera = CameraDevice.Front,
					MaxPixelDimension = 300
				};

				var file = await mediaPicker.SelectPhotoAsync (options);
				Source = ImageSource.FromStream (() => file.Source);
				imageStream = file.Source;
			} catch (Exception) {
			}
		}

		IMediaPicker mediaPicker;
		ImageSource source;
		Stream imageStream;
	}
}
