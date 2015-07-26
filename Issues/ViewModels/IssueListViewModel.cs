using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ReactiveUI;
using Akavache;
using Refit;

namespace Issues
{
	public class IssueListViewModel : BaseViewModel
	{
		public ReactiveCommand<IssueList> LoadIssues { get; private set; }
		public ReactiveCommand<IssueList> Refresh { get; private set; }

		public ReactiveCommand<object> ItemTapped { get; private set; }
		public ObservableCollection<Issue> Issues { get; private set; }

		public IssueListViewModel ()
		{
			Issues = new ObservableCollection<Issue> ();

			var canRefresh = this.WhenAny (x => x.IsBusy, x => !x.Value);
			Refresh = ReactiveCommand.CreateAsyncTask<IssueList> (canRefresh, async _ => {
				IsBusy = true;
				var issues = await LoadIssues.ExecuteAsync ();
				IsBusy = false;

				return issues;
			});

			ItemTapped = ReactiveCommand.Create ();

			LoadIssues = ReactiveCommand.CreateAsyncTask (async _ => {
				var api = RestService.For<IIssuesApi> ("http://localhost:3000/api");
				var issues = await api.GetIssues ();

				return issues;
			});
			LoadIssues.Subscribe (x => {
				Issues.Clear ();
				foreach (var i in x.issues) {
					Issues.Add (i);
				}
			});
			LoadIssues.ThrownExceptions.Subscribe (ex => UserError.Throw ("Couldn't load issues", ex));

			BlobCache.UserAccount.GetObject<string> ("email")
				.Where (x => !String.IsNullOrWhiteSpace (x))
				.InvokeCommand (this, x => x.LoadIssues);
		}
	}

	public class IssueTileViewModel : ReactiveObject
	{
		public Issue Model { get; private set; }

		public IssueTileViewModel (Issue model)
		{
			this.Model = model;
		}
	}
}
