using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ReactiveUI;
using Akavache;
using Refit;

namespace Issues
{
	public class LocationPickerViewModel : BaseViewModel
	{
		public ReactiveList<Location> Locations { get; set; }

		public ReactiveCommand<LocationList> LoadLocations { get; private set; }

		public LocationPickerViewModel ()
		{
			Locations = new ReactiveList<Location> ();

			LoadLocations = ReactiveCommand.CreateAsyncTask (async _ => {
				var api = RestService.For<IIssuesApi> ("http://localhost:3000/api");
				var locations = await api.GetLocations ();

				return locations;
			});
			LoadLocations.Subscribe (x => {
				Locations.Clear ();
				foreach (var i in x.locations) {
					Locations.Add (i);
				}
			});
			LoadLocations.ThrownExceptions.Subscribe (ex =>
				UserError.Throw ("Couldn't load locations"));

			BlobCache.UserAccount.GetObject<string> ("email")
				.Where (x => !String.IsNullOrWhiteSpace (x))
				.InvokeCommand (this, x => x.LoadLocations);
		}
	}
}
