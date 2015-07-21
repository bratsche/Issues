using System;

using Splat;
using Akavache;
using Xamarin.Forms;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Issues
{
	public class AppBootstrapper : ReactiveObject
	{
		public App App { get; private set; }

		public AppBootstrapper ()
		{
			var logger = new DebugLogger () { Level = LogLevel.Debug };
			Locator.CurrentMutable.RegisterConstant (logger, typeof(ILogger));

			App = new App ();
		}
	}

	public class App : Application
	{
		public App ()
		{
			var tabbed = new TabbedPage {
				Children = {
					//new MapPage (),
					new IssueListPage ()
				}
			};

			var nav = new NavigationPage (tabbed);

			MainPage = nav;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

