﻿using System;
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
	public enum SelectedButtonType1 {
		Normal, Medium, Urgent
	}

	public class NewIssueViewModel : BaseViewModel
	{
		public ImageSource Source {
			get { return source; }
			set { this.RaiseAndSetIfChanged (ref source, value); }
		}

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

		public NewIssueViewModel ()
		{
			var device = Resolver.Resolve<IDevice> ();
			mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;

			this.WhenAny (x => x.SelectedButton, x => x.Value)
				.Subscribe (x => {
					this.RaisePropertyChanged<NewIssueViewModel> ("NormalButtonSource");
					this.RaisePropertyChanged<NewIssueViewModel> ("MediumButtonSource");
					this.RaisePropertyChanged<NewIssueViewModel> ("UrgentButtonSource");
				});
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
