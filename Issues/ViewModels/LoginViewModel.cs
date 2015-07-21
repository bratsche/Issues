using System;

using ReactiveUI;
using Xamarin.Forms;

namespace Issues
{
	public class LoginViewModel : BaseViewModel
	{
		string email;
		public string Email {
			get { return email; }
			set { this.RaiseAndSetIfChanged (ref email, value); }
		}

		public ReactiveCommand<string> Login { get; private set; }

		public LoginViewModel ()
		{
			var canLogin = this.WhenAny (x => x.Email, x => !String.IsNullOrWhiteSpace (x.Value));
			Login = ReactiveCommand.CreateAsyncTask<string> (canLogin, async _ => {
				return email;
			});
		}
	}
}
