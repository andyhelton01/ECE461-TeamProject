using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

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


	

    public class ResponsiveMaintainer : Metric
    {

        public ResponsiveMaintainer(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 0;
            this.name = "ResponsiveMaintainer";
        }


        public override async Task Calculate()
        {	

			try {
				// FIXME name and repo needs to be parsed from url
				var client = new GitHubClient(new ProductHeaderValue("my-cool-cli"));
				var tokenAuth = new Credentials("token"); // Add environment variable for token
				client.Credentials = tokenAuth;

				// Get repo using information from Library (owner and name)
				var repo = await client.Repository.Get("andyhelton01", "ECE461-TeamProject");
				var commits = await client.Repository.Commit.GetAll(repo.Id);
				var lastCommit = commits.FirstOrDefault();

				var lastCommitDate = lastCommit.Commit.Author.Date;
				var curDate = System.DateTimeOffset.Now;
				var timeSinceLastCommit = curDate - lastCommitDate;

				this.score = (float) Math.Exp(-timeSinceLastCommit.Days);
			}catch (Octokit.AuthorizationException) {
				Library.LogError("Bad credentials. Check your access token.");
			}
        }
    }
}