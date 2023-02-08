using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using LibGit2Sharp;
using Octokit;
using Octokit.Internal;
using Octokit.GraphQL;
using static Octokit.GraphQL.Variable;
using Connection = Octokit.GraphQL.Connection;

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
                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
                client.Credentials = tokenAuth;

                // var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);
                var repo = await client.Repository.Get("pytorch", "pytorch");

                var langs = await client.Repository.GetAllLanguages(repo.Id);
                long codeSize = 0;
                foreach (RepositoryLanguage l in langs)
                {
                    codeSize += l.NumberOfBytes;
                }

                var readme = await client.Repository.Content.GetReadmeHtml(repo.Id);


                this.score = Math.Max(1500 * readme.Length / codeSize, 1);

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
                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
                client.Credentials = tokenAuth;

                var firstOneHundred = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };

                var request = new WorkflowRunsRequest { };
                // var runs = await client.Actions.Workflows.Runs.List(this.parentLibrary.owner, this.parentLibrary.name, request, firstOneHundred);
                var runs = await client.Actions.Workflows.Runs.List("pytorch", "pytorch", request, firstOneHundred);

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
				// FIXME name and repo needs to be parsed from url
				var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
				var tokenAuth = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
				client.Credentials = tokenAuth;

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

    public class BusFactor : Metric {
        public BusFactor(GitUrlLibrary parentLibrary) : base(parentLibrary) {
            this.weight = 1;
            this.name = "BusFactor";
        }

        public override async Task Calculate() {
            try {
                var productInformation = new Octokit.GraphQL.ProductHeaderValue("YOUR_PRODUCT_NAME", "YOUR_PRODUCT_VERSION");
                var connection = new Octokit.GraphQL.Connection(productInformation, Environment.GetEnvironmentVariable("GITHUB_TOKEN"));

                var query = new Query()
                    .RepositoryOwner(Var("owner"))
                    .Repository(Var("name"))
                    .Select(repo => new
                    {
                        repo.Id,
                        repo.Name,
                        repo.ForkCount,
                    }).Compile();

                var vars = new Dictionary<string, object>
                {
                    //Where owner is repo owner, and name is the name of the repo
                    // NEED TO CHANGE
                    { "owner", "andyhelton01" },
                    { "name", "ECE461-TeamProject" },
                };

                var result = await connection.Run(query, vars);
                double metricCalc = 1 - Math.Exp(-result.ForkCount / 50);
                this.score = (float)metricCalc;
            }
            catch (Octokit.AuthorizationException) {
                Library.LogError("Bad credentials. Check your access token.");
            }
        }
    }
}