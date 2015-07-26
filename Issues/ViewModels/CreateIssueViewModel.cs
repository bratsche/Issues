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

		Location location;
		public Location Location {
			get { return location; }
			set { this.RaiseAndSetIfChanged (ref location, value); }
		}

		string subject;
		public string Subject {
			get { return subject; }
			set { this.RaiseAndSetIfChanged (ref subject, value); }
		}

		string desc;
		public string Description {
			get { return desc; }
			set { this.RaiseAndSetIfChanged (ref desc, value); }
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

		public ReactiveCommand<object> SelectLocation { get; private set; }
		public ReactiveCommand<Issue> Finish { get; private set; }

		public CreateIssueViewModel ()
		{
			var device = Resolver.Resolve<IDevice> ();
			mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;

			// TODO - move this into the view layer and get rid of all these button sources,
			//        just update the images directly in the view layer.
			this.WhenAny (x => x.SelectedButton, x => x.Value)
				.Subscribe (x => {
					this.RaisePropertyChanged<CreateIssueViewModel> ("NormalButtonSource");
					this.RaisePropertyChanged<CreateIssueViewModel> ("MediumButtonSource");
					this.RaisePropertyChanged<CreateIssueViewModel> ("UrgentButtonSource");
				});

			SelectLocation = ReactiveCommand.Create ();

			// This is for our toolbar 'Add' button.
			var canFinish = this.WhenAny (
				x => x.Subject, x => x.Description, x => x.Location, x => x.PreviewSource,
				(subj, desc, loc, src) => !String.IsNullOrWhiteSpace (subj.Value) && !String.IsNullOrWhiteSpace (desc.Value) && loc.Value != null && src.Value != null
			);

			Finish = ReactiveCommand.CreateAsyncTask<Issue> (canFinish, async _ => {
				var issue = new Issue {
					location_id = Location.id,
					subject = Subject,
					description = Description
				};

				var api = RestService.For<IIssuesApi> ("http://localhost:3000/api");
				var response = await api.CreateIssue (issue);

				return await api.AddPhoto (response.id, imageStream);
			});
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
