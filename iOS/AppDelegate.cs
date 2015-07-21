using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using ReactiveUI;
using Xamarin.Forms;

namespace Issues.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public AppDelegate ()
		{
			RxApp.SuspensionHost.CreateNewAppState = () => new AppBootstrapper ();
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			RxApp.SuspensionHost.SetupDefaultSuspendResume ();

			suspendHelper = new AutoSuspendHelper (this);
			suspendHelper.FinishedLaunching (app, options);

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			var bootstrapper = RxApp.SuspensionHost.GetAppState<AppBootstrapper> ();
			LoadApplication (bootstrapper.App);

			return base.FinishedLaunching (app, options);
		}

		public override void DidEnterBackground (UIApplication application)
		{
			suspendHelper.DidEnterBackground (application);
		}

		public override void OnActivated (UIApplication application)
		{
			suspendHelper.OnActivated (application);
		}

		AutoSuspendHelper suspendHelper;
	}
}
