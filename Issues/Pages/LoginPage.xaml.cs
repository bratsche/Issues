using System;
using System.Collections.Generic;

using ReactiveUI;
using Xamarin.Forms;

namespace Issues
{
	public partial class LoginPage : ContentPage, IViewFor<LoginViewModel>
	{
		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<LoginPage, LoginViewModel> (x => x.ViewModel, default (LoginViewModel));
		public LoginViewModel ViewModel {
			get { return (LoginViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}
		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (LoginViewModel)value; }
		}

		public LoginPage (LoginViewModel viewModel)
		{
			Title = "Login";
			ViewModel = viewModel;

			InitializeComponent ();

			this.Bind (ViewModel, vm => vm.Email, v => v.EmailEntry.Text);
			this.BindCommand (ViewModel, vm => vm.Login, v => v.LoginButton);
		}
	}
}
