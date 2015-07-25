using System;
using System.Reactive.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using ReactiveUI;

namespace Issues
{
	public partial class CreateIssuePage : ContentPage, IViewFor<CreateIssueViewModel>
	{
		const string FROM_CAMERA = "Use Camera";
		const string FROM_PHOTOS = "Choose from Photos";

		public CreateIssuePage ()
		{
			InitializeComponent ();

			ViewModel = new CreateIssueViewModel ();

			this.ViewModel.SelectLocation.Subscribe (async t => {
				var picker = new LocationPickerPage ();
				picker.LocationSelected.Subscribe (loc => {
					ViewModel.Location = (Location)loc;
					Navigation.PopModalAsync ();
				});

				await Navigation.PushModalAsync (picker);
			});

			CameraImage.GestureRecognizers.Add (
				new TapGestureRecognizer (
					async (v, o) => {
						var action = await DisplayActionSheet (
							"Attach Photo",
							"Cancel",
							null,
							FROM_CAMERA,
							FROM_PHOTOS
						);

						if (action == FROM_CAMERA) {
							await ViewModel.TakePhoto ();
						} else if (action == FROM_PHOTOS) {
							await ViewModel.SelectPhoto ();
						}
					}
				)
			);

			NormalButton.Clicked += (o, e) => ViewModel.SelectedButton = SelectedButtonType.Normal;
			MediumButton.Clicked += (o, e) => ViewModel.SelectedButton = SelectedButtonType.Medium;
			UrgentButton.Clicked += (o, e) => ViewModel.SelectedButton = SelectedButtonType.Urgent;

			this.OneWayBind (ViewModel, vm => vm.NormalButtonSource, v => v.NormalButton.Image);
			this.OneWayBind (ViewModel, vm => vm.MediumButtonSource, v => v.MediumButton.Image);
			this.OneWayBind (ViewModel, vm => vm.UrgentButtonSource, v => v.UrgentButton.Image);

			this.Bind (ViewModel, vm => vm.Subject, v => v.SubjectEntry.Text);
			this.Bind (ViewModel, vm => vm.Description, v => v.DescriptionEditor.Text);
			this.OneWayBind (ViewModel, vm => vm.PreviewSource, v => v.PreviewImage.Source);

			Observable.FromEventPattern<FocusEventArgs> (x => this.LocationEntry.Focused += x, x => this.LocationEntry.Focused -= x)
				.InvokeCommand (ViewModel.SelectLocation);
		}

		public CreateIssueViewModel ViewModel {
			get { return (CreateIssueViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}

		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<CreateIssuePage, CreateIssueViewModel> (x => x.ViewModel, default (CreateIssueViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (CreateIssueViewModel)value; }
		}
	}
}
