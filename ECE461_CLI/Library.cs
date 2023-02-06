using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

		
		public virtual void addMetrics() {

			// NOTE child addMetrics should add its own addMetrics before calling this class!

			// asynchronously ask metrics to calculate
			// HACK this may blow up the computer... not sure if we need to control the number of threads here
			foreach (Metric m in metrics)
			{
				calcMetricTaskQueue.Add(m.Calculate());
			}
		}
		protected float CalculateScore()
		{ 
			if (metrics.Count == 0) {
				addMetrics();
			}

			// wait for tasks calculateTasks to finish
			waitForCalculations();

			// calculate a weighted average of all the scores of the other metrics
			float runningSum = 0;
			float divisor = 0;
			foreach (Metric m in metrics) {
				runningSum += m.weight * m.score;
				divisor += m.weight;
			}

			if (divisor == 0) divisor = 1; // avoid a divide by zero (most likely because this lib has no metrics other than netscore)

			this.score = runningSum / divisor;
			
			isCalculated = true;
			
			return this.score;
		}

		public void waitForCalculations() {
			// wait for tasks calculateTasks to finish
			foreach(Task t in calcMetricTaskQueue) {
				t.Wait();
			}

			
		}

		/*
		 * <summary> returns the NetScore of this library </summary>
		 * This will be overriden by children
		 */
		public float GetScore()
		{
			if (! isCalculated) CalculateScore();
			return score; 
		}

	
		/// <returns>a string representation of this library in JSON format</returns>
		public string ToJson()
		{
			// TODO improve this method

			if (metrics.Count == 0) {
				addMetrics();
				CalculateScore();
			}

			string jsonBlob = "{ \"libraryName\": " + this.name + ", \"libraryScore\": " + this.GetScore() + ", \"metrics\": {";
			foreach (Metric m in metrics) {
				jsonBlob += "{\"name\": " + m.name + ", " + "\"score\": " + m.score + "}, ";
			}
			jsonBlob += "}, ";
			jsonBlob += "\"type:\" " + this.GetType() + ", }";

			return jsonBlob;
		}

		public override string ToString()
		{
			return ToJson();
		}

		public static void LogError(string error_msg) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[ERROR] " + error_msg);
			Console.ForegroundColor = ConsoleColor.White;
		}


		public class LibraryComparer : IComparer<Library>
		{
			public int Compare(Library y, Library x)
			{
				
				return x.GetScore().CompareTo(y.GetScore());
			}
		}
	}




	public class UrlLibrary : Library {
		/// <summary>
		/// this is a library hosted on the internet
		/// </summary>

		private string url;

		//note: save these instances for reuse
		static HttpClient httpClient = new HttpClient();
		static JsonSerializerOptions jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

		public UrlLibrary(string url) : base(url) {
			this.url = url;

			// TODO give this lib a better name please

		

		}

		
		public virtual string GetUrl() {
			return url;
		}
		

		private async static Task<string> scrapeForGitUrl(string url) {
				
			// TODO get package name from url
			string[] phrases = url.Split("/");
			string packageName = phrases[phrases.Length-1];

			using var client = new HttpClient();

			var result = await client.GetStringAsync("https://registry.npmjs.org/" + packageName);

			// HACK this may be the least robust possible way of doing this 
			string[] tokens = result.Split("\"");
			foreach (string s in tokens) {
				if (s.Contains("github.com")) {
					return s;
				}
			}

			return "no_url_found";
			
		}

		public static Library GetFromNpmUrl(string url) {

			// TODO scrapeForGitUrl should return a Task<string> with the git url
			Task<string> urlScrape = scrapeForGitUrl(url);
			urlScrape.Wait();
			string gitUrl = urlScrape.Result;
			// string gitUrl = "https://github.com/andyhelton01/ECE461-TeamProject"; // TODO this should be from scrapeForGitUrl
			if (gitUrl == "no_url_found") {
				return new NPMUrlLibrary(url);
			}else{
				return new NPMGitUrlLibrary(url, gitUrl);
			}
		}

		


		
	}

	public class GitUrlLibrary : UrlLibrary {
		/// <summary>
		/// this is a library that is hosted on github
		/// </summary>
		public GitUrlLibrary(string url) : base(url) {
			
			// hit api and download all needed data
			// TODO

			
		}

		
		public override void addMetrics() {



            // add metrics to metric list 
            metrics.Add(new RampUp(this));
            metrics.Add(new Correctness(this));
            //metrics.Add(new BusFactor(this));
            metrics.Add(new ResponsiveMaintainer(this));
            //metrics.Add(new License(this));

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

	public class NPMGitUrlLibrary : GitUrlLibrary {
		/// <summary>
		/// this is a library with a npm url but is still hosted on github
		/// </summary>

		public string npmUrl;
		public NPMGitUrlLibrary(string npmUrl, string gitUrl) : base(gitUrl) {
			this.npmUrl = npmUrl;
		}

		public override string GetUrl() {
			return npmUrl;
		}
	}
}
