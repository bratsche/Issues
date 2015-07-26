using System;
using System.Linq;

using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Issues
{
	public class LocationPickerPage : ContentPage, IViewFor<LocationPickerViewModel>
	{
		public ReactiveCommand<object> LocationSelected { get; private set; }

		public LocationPickerPage ()
		{
			ViewModel = new LocationPickerViewModel ();

			LocationSelected = ReactiveCommand.Create ();

			var map = new Map ();

			ViewModel.Locations.ItemsAdded.Subscribe (x => {
				var pin = new Pin {
					Label = x.name,
					Type = PinType.Place,
					Position = new Position (x.latitude, x.longitude),
					BindingContext = x
				};

				pin.Clicked += (sender, e) => {
					var location = ((sender as Pin).BindingContext as Location);
					LocationSelected.Execute (location);
				};

				if (map.Pins.Count == 0) {
					map.MoveToRegion (MapSpan.FromCenterAndRadius (pin.Position, Distance.FromMiles (0.25)));
				}

				map.Pins.Add (pin);
			});

			Content = new StackLayout {
				Padding = new Thickness (20, 20, 20, 20),
				Children = {
					map
				}
			};
		}

		public LocationPickerViewModel ViewModel {
			get { return (LocationPickerViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}

		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<LocationPickerPage, LocationPickerViewModel> (x => x.ViewModel, default (LocationPickerViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (LocationPickerViewModel)value; }
		}
	}
}
