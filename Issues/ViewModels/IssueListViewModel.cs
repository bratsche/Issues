using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
		[DataMember] public ReactiveList<Issue> Issues { get; private set; }
		[DataMember] public IReactiveDerivedList<IssueTileViewModel> IssueTiles { get; private set; }

		public IssueListViewModel ()
		{
			Issues = new ReactiveList<Issue> ();

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
				Issues.AddRange (x.issues);
				Issues.Add (new Issue { subject = "Test", description = "foobar", id = 5 });

				System.Diagnostics.Debug.WriteLine ("There are {0} items in Issues", Issues.Count);
				System.Diagnostics.Debug.WriteLine ("and {0} in IssueTiles", IssueTiles.Count);
			});
			LoadIssues.ThrownExceptions.Subscribe (ex => UserError.Throw ("Couldn't load issues", ex));

			IssueTiles = Issues.CreateDerivedCollection (
				x => new IssueTileViewModel (x),
				x => !String.IsNullOrWhiteSpace (x.subject),
				(x, y) => x.Model.id.CompareTo (y.Model.id)
			);

			BlobCache.UserAccount.GetObject<string> ("email")
				.Where (x => !String.IsNullOrWhiteSpace (x))
				.InvokeCommand (this, x => x.LoadIssues);
		}
	}

	[DataContract]
	public class IssueTileViewModel : ReactiveObject
	{
		[DataMember]
		public Issue Model { get; private set; }

		public IssueTileViewModel (Issue model)
		{
			this.Model = model;
		}
	}
}
