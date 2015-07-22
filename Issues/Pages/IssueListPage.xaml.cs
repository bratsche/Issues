using System;
using System.Linq;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Collections.Generic;

using Splat;
using Akavache;
using ReactiveUI;
using Xamarin.Forms;

namespace Issues
{
	public partial class IssueListPage : ContentPage, IViewFor<IssueListViewModel>
	{
		public const string ItemTappedCommandPropertyName = "ItemTappedCommand";
		public static BindableProperty ItemTappedCommandProperty = BindableProperty.Create (
			"ItemTappedCommand",
			typeof (ICommand),
			typeof (IssueListPage),
			null
		);
		public ICommand ItemTappedCommand
		{
			get { return (ICommand)GetValue (ItemTappedCommandProperty); }
			set { SetValue (ItemTappedCommandProperty, value); }
		}

		public ReactiveCommand<string> LoadEmail { get; private set; }

		public IssueListPage ()
		{
			InitializeComponent ();

			ViewModel = new IssueListViewModel ();

			this.Title = "Issues";

			LoadEmail = ReactiveCommand.CreateAsyncTask<string> (async _ => {
				var email = await BlobCache.UserAccount.GetOrFetchObject (
					"email",
					async () => {
						var vm = new LoginViewModel ();
						await Navigation.PushModalAsync (new LoginPage (vm));

						var address = await vm.Login.FirstAsync ();

						await Navigation.PopModalAsync ();

						return address;
					}
				);

				return email;
			});

			this.OneWayBind (ViewModel, vm => vm.Issues, v => v.IssueTiles.ItemsSource);
			this.OneWayBind (ViewModel, vm => vm.ItemTapped, v => v.ItemTappedCommand);
			this.OneWayBind (ViewModel, vm => vm.Refresh, v => v.IssueTiles.RefreshCommand);
			this.Bind (ViewModel, vm => vm.IsBusy, v => v.IssueTiles.IsRefreshing);
		}
			
//		protected override void OnBindingContextChanged ()
//		{
//			base.OnBindingContextChanged ();
//
//			RemoveBinding (ItemTappedCommandProperty);
//			SetBinding (ItemTappedCommandProperty, new Binding (ItemTappedCommandPropertyName));
//		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			LoadEmail.Execute (null);
		}

		void HandleItemTapped (object sender, ItemTappedEventArgs e)
		{
			var cmd = ItemTappedCommand;

			if (cmd != null && cmd.CanExecute (e.Item)) {
				cmd.Execute (e.Item);
			}

			this.IssueTiles.SelectedItem = null;
		}

		public IssueListViewModel ViewModel {
			get { return (IssueListViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}

		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<IssueListPage, IssueListViewModel> (x => x.ViewModel, default (IssueListViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (IssueListViewModel)value; }
		}
	}
}
