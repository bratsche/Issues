using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ReactiveUI;
using Refit;

namespace Issues
{
	public class LocationPickerViewModel : BaseViewModel
	{
		public ObservableCollection<Location> Locations { get; set; }

		public LocationPickerViewModel ()
		{
			Locations = new ObservableCollection<Location> {
				new Location { id = 0, name = "Milton S Eisenhower Library", latitude = 39.3289939f, longitude = -76.6194905f },
				new Location { id = 1, name = "Levering Hall", latitude = 39.3289899f, longitude = -76.621592f }
			};
		}
	}
}
