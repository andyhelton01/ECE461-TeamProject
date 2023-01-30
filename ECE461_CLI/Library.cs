using System;
using System.Collections.Generic;
using System.Text;

namespace ECE461_CLI
{
	
	public class Library
	{

		public List<Metric> metrics = new List<Metric>();

		public NetScore netScore;
		public Library()
		{

		}

		protected float CalculateScore()
		{ 
			foreach (Metric m in metrics)
			{
				m.Calculate();
			}

			netScore = new NetScore(this);

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
			return "TODO"; // TODO complete this
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

		public UrlLibrary(string url) {
			this.url = url;
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
