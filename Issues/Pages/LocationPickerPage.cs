using System;
using System.Linq;

using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Issues
{
	public class LocationPickerPage : ContentPage, IViewFor<LocationPickerViewModel>
	{
		//public event EventHandler<Location> LocationSelected;
		public ReactiveCommand<object> LocationSelected { get; private set; }

		public LocationPickerPage ()
		{
			ViewModel = new LocationPickerViewModel ();

			LocationSelected = ReactiveCommand.Create ();

			var map = BuildMap ();

			Content = new StackLayout {
				Padding = new Thickness (20, 20, 20, 20),
				Children = {
					map
				}
			};
		}

		Map BuildMap ()
		{
			var pins = ViewModel.Locations.Select (l => new Pin { Label = l.name, Type = PinType.Place, Position = new Position (l.latitude, l.longitude), BindingContext = l });

			var map = new Map (MapSpan.FromCenterAndRadius (pins.First().Position, Distance.FromMiles (0.25)));
			map.IsShowingUser = true;

			foreach (var p in pins) {
				p.Clicked += (sender, e) => {
					var loc = ((sender as Pin).BindingContext as Location);
					LocationSelected.Execute (loc);
//					if (LocationSelected != null) {
//						var loc = ((sender as Pin).BindingContext as Location);
//						LocationSelected (this, loc);
//					}
				};
				map.Pins.Add (p);
			}

			return map;
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
