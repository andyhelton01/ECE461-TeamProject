using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using LibGit2Sharp;
using Octokit;
using Octokit.Internal;

namespace ECE461_CLI
{
	public abstract class Metric
	{
		public GitUrlLibrary parentLibrary;

		public float score;

		public string name;

		public float weight; // must be overridden by child class

		public Metric(GitUrlLibrary parentLibrary)
		{
			this.parentLibrary = parentLibrary;

		}


		/*
		 * <summary> calculates the score for this metric
		 */
		public abstract Task Calculate();



	}

    public class RampUp : Metric
    {

        public RampUp(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "RampUp";
        }

        private float Sigmoid(float x)
        {
            return 1 / (1 + (float)Math.Exp(-x));
        }

        public override async Task Calculate()
        {

            try
            {

                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0) {
                    Library.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }
                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);
                // var repo = await client.Repository.Get("pytorch", "pytorch");

                var langs = await client.Repository.GetAllLanguages(repo.Id);
                long codeSize = 0;
                foreach (RepositoryLanguage l in langs)
                {
                    codeSize += l.NumberOfBytes;
                }

                var readme = await client.Repository.Content.GetReadmeHtml(repo.Id);

                if (codeSize == 0) {
                    Library.LogError("repository " + this.parentLibrary.name + " has a code size of zero");
                    this.score = 0;
                
                }else{
                    this.score = Math.Min(1500 * readme.Length / codeSize, 1);
                }

                

            }
            catch (Octokit.AuthorizationException)
            {
                Library.LogError("Bad credentials. Check your access token.");
            }
        }
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
                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0) {
                    Library.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }

                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var firstOneHundred = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };

                var request = new WorkflowRunsRequest { };
                var runs = await client.Actions.Workflows.Runs.List(this.parentLibrary.owner, this.parentLibrary.name, request, firstOneHundred);
                // var runs = await client.Actions.Workflows.Runs.List("pytorch", "pytorch", request, firstOneHundred);

                float score = 0;
                int count = 0;
                foreach (WorkflowRun r in runs.WorkflowRuns)
                {
                    switch (r.Status.ToString())
                    {
                        case "completed":
                        case "success":
                            score += 1;
                            break;
                        case "in_progress":
                        case "queued":
                        case "pending":
                            score += (float) 0.7;
                            break;
                        case "neutral":
                        case "skipped":
                        case "cancelled":
                        case "stale":
                        case "action_required":
                            score += (float)0.5;
                            break;
                    }
                    count++;
                }

                if (count == 0) {
                    this.score = (float)0.4;
                } else
                {
                    this.score = score / count;
                }
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
                
                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0) {
                    Library.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }
                
				// FIXME name and repo needs to be parsed from url
				var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
				var tokenAuth = new Octokit.Credentials(access_token);
				client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);
                // var repo = await client.Repository.Get("pytorch", "pytorch");
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