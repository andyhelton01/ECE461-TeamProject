using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace ECE461_CLI
{

	public class Library
	{



		public List<Metric> metrics = new List<Metric>();

		public List<Task> calcMetricTaskQueue = new List<Task>();

		public float score;

		public string name;



		bool isCalculated = false;
		public Library(string name)
		{
			this.name = name;
		}


		public virtual void addMetrics()
		{

			// NOTE child addMetrics should add its own addMetrics before calling this class!

			// asynchronously ask metrics to calculate
			// HACK this may blow up the computer... not sure if we need to control the number of threads here
			foreach (Metric m in metrics)
			{
				try
				{
					calcMetricTaskQueue.Add(m.Calculate());
				}
				catch (Exception e)
				{
					Program.LogError("An unexpected Exception Occured. Please check your URL_FILE, and the validity of your repos." + e.ToString());


				}

			}
		}
		protected float CalculateScore()
		{
			if (metrics.Count == 0)
			{
				addMetrics();
			}

			// wait for tasks calculateTasks to finish
			waitForCalculations();

			// calculate a weighted average of all the scores of the other metrics
			float runningSum = 0;
			float divisor = 0;
			foreach (Metric m in metrics)
			{
				runningSum += m.score == -1 ? 0 : m.weight * m.score;
				divisor += m.weight;
			}

			if (divisor == 0) divisor = 1; // avoid a divide by zero (most likely because this lib has no metrics other than netscore)

			this.score = runningSum / divisor;

			isCalculated = true;

			return this.score;
		}

		public void waitForCalculations()
		{
			// wait for tasks calculateTasks to finish
			foreach (Task t in calcMetricTaskQueue)
			{
				try
				{
					t.Wait(TimeSpan.FromSeconds(Program.REQUEST_TIMEOUT_TIME));

				}
				catch (Exception e)
				{
					Program.LogError("An unexpected Exception Occured. Please check your URL_FILE, and the validity of your repos." + e.ToString());

				}
			}


		}

		/*
		 * <summary> returns the NetScore of this library </summary>
		 * This will be overriden by children
		 */
		public float GetScore()
		{
			if (!isCalculated) CalculateScore();
			return score;
		}


		/// <returns>a string representation of this library in JSON format</returns>
		public string ToJson()
		{


			if (metrics.Count == 0)
			{
				addMetrics();
				CalculateScore();
			}

			string jsonBlob = "{ \"libraryName\": " + this.name + ", \"libraryScore\": " + Math.Round(this.GetScore(), 2) + ", \"metrics\": {";
			foreach (Metric m in metrics)
			{
				jsonBlob += "{\"name\": " + m.name + ", " + "\"score\": " + m.GetScore() + "}, ";
			}
			jsonBlob += "}, ";
			jsonBlob += "\"type:\" " + this.GetType() + ", }";

			return jsonBlob;
		}

		/// <returns>a string representation of this library based on project specifications</returns>
		public virtual string ToOutput()
		{
			// since this is not a urlLibrary, we will be missing a lot of values

			if (metrics.Count == 0)
			{ // ensure we are ready to be outputted
				addMetrics();
				CalculateScore();
			}

			string jsonBlob = "{\"URL\":\"\", \"NET_SCORE\":" + Math.Round(this.GetScore(), 2) + ", \"RAMP_UP_SCORE\":-1, \"CORRECTNESS_SCORE\":-1, \"BUS_FACTOR_SCORE\":-1, \"RESPONSIVE_MAINTAINER_SCORE\":-1, \"LICENSE_SCORE\":-1}";

			return jsonBlob;
		}

		public override string ToString()
		{
			return ToJson();
		}




		public class LibraryComparer : IComparer<Library>
		{
			public int Compare(Library y, Library x)
			{

				return x.GetScore().CompareTo(y.GetScore());
			}
		}
	}




	public class UrlLibrary : Library
	{
		/// <summary>
		/// this is a library hosted on the internet
		/// </summary>

		private string url;

		//note: save these instances for reuse
		static HttpClient httpClient = new HttpClient();
		static JsonSerializerOptions jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

		public UrlLibrary(string url) : base(url)
		{
			this.url = url;

			// TODO give this lib a better name please



		}


		public virtual string GetUrl()
		{
			return url;
		}

		public override string ToOutput()
		{
			if (metrics.Count == 0)
			{ // ensure we are ready to be outputted
				addMetrics();
				CalculateScore();
			}

			string jsonBlob = "{ \"URL\":\"" + this.GetUrl() + "\", \"NET_SCORE\":" + Math.Round(this.GetScore(), 2);
			foreach (Metric m in metrics)
			{
				jsonBlob += ", \"" + m.name + "\":" + m.GetScore();
			}
			jsonBlob += "}";

			return jsonBlob;
		}


		private async static Task<string> scrapeForGitUrl(string url)
		{

			// get package name from url
			string[] phrases = url.Split("/");
			string packageName = phrases[phrases.Length - 1];

			using var client = new HttpClient();

			var result = await client.GetStringAsync("https://registry.npmjs.org/" + packageName);

			// HACK this may be the least robust possible way of doing this 
			string[] tokens = result.Split("\"");
			foreach (string s in tokens)
			{
				if (s.Contains("github.com"))
				{
					return s;
				}
			}

			return "no_url_found";

		}

		public static Library GetFromNpmUrl(string url)
		{

			Task<string> urlScrape = scrapeForGitUrl(url);

			try
			{
				urlScrape.Wait(TimeSpan.FromSeconds(Program.REQUEST_TIMEOUT_TIME));
			}
			catch (AggregateException)
			{ // probably a 404 error
				Program.LogError("Invalid library url: " + url);
				return null;
			}
			string gitUrl = urlScrape.Result;

			if (gitUrl == "no_url_found")
			{
				return new NPMUrlLibrary(url);
			}
			else
			{
				return new NPMGitUrlLibrary(url, gitUrl);
			}
		}





	}

	public class GitUrlLibrary : UrlLibrary
	{
		/// <summary>
		/// this is a library that is hosted on github
		/// </summary>

		public string owner { get; }


		public GitUrlLibrary(string url) : base(url)
		{



			// get the user name and repository name
			string[] phrases = url.Split("/");
			if (phrases.Length <= 2)
			{
				Program.LogError("Invalid github url: " + url);
				this.owner = "invalid";
				this.name = "invalid";
			}
			else
			{
				this.owner = phrases[phrases.Length - 2];
				this.name = phrases[phrases.Length - 1];
				if (this.name.Contains(".git"))
				{
					this.name = this.name.Substring(0, this.name.Length - 4);
				}
			}
			// Console.WriteLine("Url: " + url);
			// Console.WriteLine("Usrname: " + this.username + ", reponame: " + this.reponame);
		}


		public override void addMetrics()
		{



			// add metrics to metric list 
			metrics.Add(new RampUp(this));
			metrics.Add(new Correctness(this));
			metrics.Add(new BusFactor(this));
			metrics.Add(new ResponsiveMaintainer(this));
			metrics.Add(new LicenseMetric(this));

			base.addMetrics();
		}

	}

	public class NPMUrlLibrary : UrlLibrary
	{
		/// <summary>
		/// this is a library that is hosted on npm
		/// </summary>
		public NPMUrlLibrary(string url) : base(url)
		{

			// hit api and download all needed data
			// TODO

			// add metrics to metric list 

		}

	}

	public class NPMGitUrlLibrary : GitUrlLibrary
	{
		/// <summary>
		/// this is a library with a npm url but is still hosted on github
		/// </summary>

		public string npmUrl;
		public NPMGitUrlLibrary(string npmUrl, string gitUrl) : base(gitUrl)
		{
			this.npmUrl = npmUrl;
		}

		public override string GetUrl()
		{
			return npmUrl;
		}
	}
}
