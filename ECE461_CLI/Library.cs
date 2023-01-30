using System;
using System.Collections.Generic;
using System.Text;

namespace ECE461_CLI
{
	
	public class Library
	{

		public List<Metric> metrics = new List<Metric>();

		public NetScore netScore;

		public string name;
		public Library(string name)
		{
			this.name = name;
		}

		
		protected void addMetrics() {
			// NOTE child addMetrics should add its own addMetrics before calling this class!

			netScore = new NetScore(this);
			metrics.Add(netScore);
		}
		protected float CalculateScore()
		{ 
			if (metrics.Count == 0) {
				addMetrics();
			}

			
			foreach (Metric m in metrics)
			{
				
				m.Calculate();
			}

			

			netScore.Calculate();
			return netScore.score;
		}

		/*
		 * <summary> returns the NetScore of this library </summary>
		 * This will be overriden by children
		 */
		public float GetScore()
		{
			return 0; 
		}

	
		/// <returns>a string representation of this library in JSON format</returns>
		public string ToJson()
		{
			// TODO improve this method

			if (metrics.Count == 0) {
				addMetrics();
				CalculateScore();
			}

			string jsonBlob = "{ \"libraryName\": " + this.name + ", \"metrics\": {";
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
	}


	public class UrlLibrary : Library {
		/// <summary>
		/// this is a library hosted on the internet
		/// </summary>

		public string url;

		public UrlLibrary(string url) : base(url) {
			this.url = url;

			// TODO give this lib a better name please

		}

		
	}

	public class GitUrlLibrary : UrlLibrary {
		/// <summary>
		/// this is a library that is hosted on github
		/// </summary>
		public GitUrlLibrary(string url) : base(url) {
			
			// hit api and download all needed data
			// TODO

			// add metrics to metric list 
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
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
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
			// metrics.Add(new Correctness(this));
		}

	}
}
