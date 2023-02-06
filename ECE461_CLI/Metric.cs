﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Octokit;
using Octokit.Internal;

namespace ECE461_CLI
{
	public abstract class Metric
	{
		public Library parentLibrary;

		public float score;

		public string name;

		public float weight; // must be overridden by child class

		public Metric(Library parentLibrary)
		{
			this.parentLibrary = parentLibrary;

		}


		/*
		 * <summary> calculates the score for this metric
		 */
		public abstract Task Calculate();



	}

    public class Correctness : Metric
    {

        public Correctness(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "Correctness";
        }


        public override async Task Calculate()
        {

            try
            {
                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
                client.Credentials = tokenAuth;

                // Get repo using information from Library (owner and name)
                // var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);
                //var runs = await client.Actions.Workflows.Runs.List("octokit", "octokit.net");
                //Console.WriteLine(runs.TotalCount);
                
                this.score = 1;
            }
            catch (Octokit.AuthorizationException)
            {
                Library.LogError("Bad credentials. Check your access token.");
            }
        }
    }

    public class ResponsiveMaintainer : Metric
    {

        public ResponsiveMaintainer(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "ResponsiveMaintainer";
        }


        public override async Task Calculate()
        {	

			try {
				// FIXME name and repo needs to be parsed from url
				var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
				var tokenAuth = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
				client.Credentials = tokenAuth;

                // Get repo using information from Library (owner and name)
                // var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);
                var repo = await client.Repository.Get("pytorch", "pytorch");
                var firstOneHundred = new ApiOptions
                {
                    PageSize = 100,
                    PageCount = 1
                };
                var commits = await client.Repository.Commit.GetAll(repo.Id, firstOneHundred);
                
                if (commits.Count == 0)
                {
                    this.score = 0;
                }
                else
                {
                    var lastCommit = commits.FirstOrDefault();

                    var lastCommitDate = lastCommit.Commit.Author.Date;
                    var curDate = System.DateTimeOffset.Now;
                    var timeSinceLastCommit = curDate - lastCommitDate;

                    this.score = (float)Math.Exp(-0.1 * timeSinceLastCommit.Days);
                }

            }
            catch (Octokit.AuthorizationException) {
				Library.LogError("Bad credentials. Check your access token.");
			}
        }
    }
}