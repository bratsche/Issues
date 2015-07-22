using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ReactiveUI;

namespace Issues
{
	public partial class IssueTileView : ContentView, IViewFor<IssueTileViewModel>
	{
		public IssueTileView ()
		{
			InitializeComponent ();

			this.OneWayBind (ViewModel, vm => vm.Model.subject, v => v.Subject.Text);
		}

		public IssueTileViewModel ViewModel {
			get { return (IssueTileViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}

		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<IssueTileView, IssueTileViewModel> (x => x.ViewModel, default (IssueTileViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (IssueTileViewModel)value; }
		}
	}
}
