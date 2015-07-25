using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
	public enum SelectedButtonType {
		Normal, Medium, Urgent
	}

	public class CreateIssueViewModel : BaseViewModel
	{
		public ImageSource PreviewSource {
			get { return source; }
			set { this.RaiseAndSetIfChanged (ref source, value); }
		}

		public string Subject { get; set; }
		public string Description { get; set; }

		SelectedButtonType? selected_button;
		public SelectedButtonType? SelectedButton {
			get { return selected_button; }
			set { this.RaiseAndSetIfChanged (ref selected_button, value); }
		}

		public FileImageSource NormalButtonSource {
			get { return SelectedButton == SelectedButtonType.Normal ? "normal-filled.png" : "normal-empty.png"; }
		}

		public FileImageSource MediumButtonSource {
			get { return SelectedButton == SelectedButtonType.Medium ? "medium-filled.png" : "medium-empty.png"; }
		}

		public FileImageSource UrgentButtonSource {
			get { return SelectedButton == SelectedButtonType.Urgent ? "urgent-filled.png" : "urgent-empty.png"; }
		}

		public ReactiveCommand<object> SelectLocation { get; private set; }
		public ReactiveCommand<object> NormalSelected { get; private set; }
		public ReactiveCommand<object> MediumSelected { get; private set; }
		public ReactiveCommand<object> UrgentSelected { get; private set; }

		public CreateIssueViewModel ()
		{
			var device = Resolver.Resolve<IDevice> ();
			mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;

			this.WhenAny (x => x.SelectedButton, x => x.Value)
				.Subscribe (x => {
					this.RaisePropertyChanged<CreateIssueViewModel> ("NormalButtonSource");
					this.RaisePropertyChanged<CreateIssueViewModel> ("MediumButtonSource");
					this.RaisePropertyChanged<CreateIssueViewModel> ("UrgentButtonSource");
				});

			SelectLocation = ReactiveCommand.Create ();
//			RadioSelected = ReactiveCommand.CreateAsyncTask<string> (async s => {
//				System.Diagnostics.Debug.WriteLine ("RADIO {0}", s);
//				return "foo";
//			});

			NormalSelected = ReactiveCommand.Create ();
			NormalSelected.Subscribe (t => SelectedButton = SelectedButtonType.Normal);

//			SelectLocation.Subscribe (t => {
//				System.Diagnostics.Debug.WriteLine ("SELECT LOCATION");
//			});
		}

		public async Task<MediaFile> TakePhoto ()
		{
			PreviewSource = null;

			var options = new CameraMediaStorageOptions { DefaultCamera = CameraDevice.Rear, MaxPixelDimension = 300 };
			return await mediaPicker.TakePhotoAsync (options).ContinueWith (t => {
				if (!t.IsFaulted && !t.IsCanceled) {
					var mediaFile = t.Result;

					PreviewSource = ImageSource.FromStream (() => mediaFile.Source);
					imageStream = mediaFile.Source;

					return mediaFile;
				}

				return null;
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		public async Task SelectPhoto ()
		{
			PreviewSource = null;

			try {
				var options = new CameraMediaStorageOptions {
					DefaultCamera = CameraDevice.Front,
					MaxPixelDimension = 300
				};

				var file = await mediaPicker.SelectPhotoAsync (options);
				PreviewSource = ImageSource.FromStream (() => file.Source);
				imageStream = file.Source;
			} catch (Exception) {
			}
		}

		IMediaPicker mediaPicker;
		ImageSource source;
		Stream imageStream;
	}
}
