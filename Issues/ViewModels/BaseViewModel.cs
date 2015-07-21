using System;

using ReactiveUI;

namespace Issues
{
	public class BaseViewModel : ReactiveObject
	{
		public LoginPage View { get; set; }

		bool busy;
		public bool IsBusy {
			get { return busy; }
			set { this.RaiseAndSetIfChanged (ref busy, value); }
		}
	}
}
