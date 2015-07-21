using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Issues
{
	public interface IIssuesApi
	{
		[Get("/locations")]
		Task<LocationList> GetLocationsRaw ();

		[Get("/issues")]
		Task<IssueList> GetIssuesRaw ();

		[Post("/issues")]
		Task<Issue> CreateIssueRaw ([Body] Issue issue);

		[Multipart]
		[Post("/issues/{id}/photo")]
		Task<Issue> AddPhoto (int id, [AttachmentName ("image.jpg")] Stream stream);
	}

	public static class IssuesApiExtensions
	{
		public static async Task<LocationList> GetLocations (this IIssuesApi This)
		{
			var ret = await This.GetLocationsRaw ();

			return ret;
		}

		public static async Task<IssueList> GetIssues (this IIssuesApi This)
		{
			var ret = await This.GetIssuesRaw ();

			return ret;
		}
	}

	public enum Urgency
	{
		Normal,
		Medium,
		Emergency
	}

	public class Issue
	{
		public int id { get; set; }
		public Urgency urgency { get; set; }
		public int location_id { get; set; }
		public string subject { get; set; }
		public string description { get; set; }
	}

	public class IssueList
	{
		public List<Issue> issues { get; set; }
	}

	public class Location
	{
		public int id { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string name { get; set; }
	}

	public class LocationList
	{
		public List<Location> locations { get; set; }
	}
}
