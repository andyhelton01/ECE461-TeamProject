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
        public GitUrlLibrary parentLibrary;

        public float score;

        public string name;

        public float weight; // must be overridden by child class

        public Metric(GitUrlLibrary parentLibrary)
        {
            this.parentLibrary = parentLibrary;

        }
        public float GetScore()
        {
            return (float)Math.Round(this.score, 2);
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
            this.name = "RAMP_UP_SCORE";
        }

        private float sigmoid(float x)
		{
            return 1 / (1 + (float) Math.Exp(-x));
		}

        public override async Task Calculate()
        {

            try
            {

                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0)
                {
                    Program.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }
                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new Octokit.ProductHeaderValue("ECE461_CLI"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);

                var langs = await client.Repository.GetAllLanguages(repo.Id);
                long codeSize = 0;
                foreach (RepositoryLanguage l in langs)
                {
                    codeSize += l.NumberOfBytes;
                }

                // var readme = await client.Repository.Content.GetReadmeHtml(repo.Id);
                string readme = (await client.Repository.Content.GetReadme(repo.Id)).Content;

                if (codeSize == 0)
                {
                    Program.LogError("repository " + this.parentLibrary.owner + "/" + this.parentLibrary.name + " has a code size of zero");
                    this.score = 0;

                }
                else
                {
                    float ratio = (float)readme.Length / 10000;// / (float)Math.Pow(codeSize,2.0/3.0);
           
                   
                    // this.score = Math.Min(1500 * readme.Length / codeSize, 1);
                    this.score = 2*(sigmoid(ratio)-0.5F);
                    Console.WriteLine("ratio: " + ratio);
                    Console.WriteLine("score: " + score);
                }



            }
            catch (Octokit.AuthorizationException)
            {
                Program.LogError("Bad credentials. Check your access token.");
            }
            catch (Octokit.NotFoundException)
            {
                Program.LogError("Non existent repository");
            }
        }
    }

    public class Correctness : Metric
    {

        public Correctness(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "CORRECTNESS_SCORE";
        }


        public override async Task Calculate()
        {

            try
            {
                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0)
                {
                    Program.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }

                // FIXME name and repo needs to be parsed from url
                var client = new GitHubClient(new Octokit.ProductHeaderValue("ECE461_CLI"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var firstOneHundred = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };

                var request = new WorkflowRunsRequest { };
                var runs = await client.Actions.Workflows.Runs.List(this.parentLibrary.owner, this.parentLibrary.name, request, firstOneHundred);

                float score = 0;
                int count = 0;
                foreach (WorkflowRun r in runs.WorkflowRuns)
                {
                    switch (r.Conclusion.ToString())
                    {
                        case "failure":
                        case "timed_out":
                            break;
                        case "success":
                        case "completed":
                            score += 1;
                            break;
                        case "in_progress":
                        case "queued":
                        case "pending":
                            score += (float)0.7;
                            break;
                        case "neutral":
                        case "skipped":
                        case "cancelled":
                        case "stale":
                        case "action_required":
                            score += (float)0.5;
                            break;
                        default:
                            break;
                    }
                    count++;
                }

                if (count == 0)
                {
                    this.score = (float)0.4;
                }
                else
                {
                    this.score = score / count;
                }
            }
            catch (Octokit.AuthorizationException)
            {
                Program.LogError("Bad credentials. Check your access token.");
            }
            catch (Octokit.NotFoundException)
            {
                Program.LogError("Non existent repository");
            }
        }
    }

    public class ResponsiveMaintainer : Metric
    {

        public ResponsiveMaintainer(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 2;
            this.name = "RESPONSIVE_MAINTAINER_SCORE";
        }


        public override async Task Calculate()
        {

            try
            {

                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0)
                {
                    Program.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }


                var client = new GitHubClient(new Octokit.ProductHeaderValue("ECE461_CLI"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);

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

                    this.score = (float)Math.Exp(-0.01 * timeSinceLastCommit.Days);

                }
            }
            catch (Octokit.AuthorizationException)
            {
                Program.LogError("Bad credentials. Check your access token.");
            }
            catch (Octokit.NotFoundException)
            {
                Program.LogError("Non existent repository");
            }
        }
    }

    public class BusFactor : Metric
    {
        public BusFactor(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "BUS_FACTOR_SCORE";
        }

        public override async Task Calculate()
        {
            try
            {

                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0)
                {
                    Program.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }

                var productInformation = new Octokit.GraphQL.ProductHeaderValue("YOUR_PRODUCT_NAME", "YOUR_PRODUCT_VERSION");
                var connection = new Connection(productInformation, access_token);

                var client = new GitHubClient(new Octokit.ProductHeaderValue("my-cool-cli"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);

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
                    { "owner", repo.Owner.Login },
                    { "name", repo.Name },
                };

                var result = await connection.Run(query, vars);
                double metricCalc = 1 - Math.Exp(-(float)result.ForkCount / 200);
                this.score = (float)metricCalc;
            }
            catch (Octokit.AuthorizationException)
            {
                Program.LogError("Bad credentials. Check your access token.");
            }
            catch (Octokit.NotFoundException)
            {
                Program.LogError("Non existent repository");
            }
        }
    }
    public class LicenseMetric : Metric
    {
        
        /// The list of compatible licenses with this project
        string[] compatibleLicenses = {"mit", "lgpl 2.1", "lgpl 2.1+","bsd", "bsd-new", "x11", "public domain"};

        string[] incompatibleLicenses = {"gplv2", "gplv2+", "gplv3", "gplv3+", "affero gplv3", "apache2.0", "mpl", "mpl 1.1"};

        public LicenseMetric(GitUrlLibrary parentLibrary) : base(parentLibrary)
        {
            this.weight = 1;
            this.name = "LICENSE_SCORE";
        }


        public override async Task Calculate()
        {

            try
            {

                string access_token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

                if (access_token is null || access_token.Length == 0)
                {
                    Program.LogError("access token not set. Ensure the env variable GITHUB_TOKEN is set");
                    return;
                }
               
                var client = new GitHubClient(new Octokit.ProductHeaderValue("ECE461_CLI"));
                var tokenAuth = new Octokit.Credentials(access_token);
                client.Credentials = tokenAuth;

                var repo = await client.Repository.Get(this.parentLibrary.owner, this.parentLibrary.name);

             
            
                string readme = await client.Repository.Content.GetReadmeHtml(repo.Id);
                //string readme = (await client.Repository.Content.GetReadme(repo.Id)).Content;

                // Console.WriteLine(readme);

                string[] readmeLines = readme.Split("<");

                List<string> licenseLines = new List<string>();

                // search for all lines that mention a license
                foreach (string line in readmeLines) {
                    if (line.ToLower().Contains("license")) licenseLines.Add(line.ToLower());
                }

                if (licenseLines.Count > 0) {
                    // we have found lines mentioning a license, now we need to see whether it is compatible or not
                    score = 0.5F;
                    
                    // search through all lines that could contain the license
                    foreach (string line in licenseLines)
					{
                        // Program.LogDebug("Searching line " + line + "for licenses");
                        // if we found a compatible license, increase the score
                        foreach (string license in compatibleLicenses)
						{
                            if (line.Contains(license))
							{
                                Program.LogDebug("found compatible license: " + license);
                                score += 0.5F;
							}
						}

                        // if we found an incompatible license, decrease the score
                        foreach(string license in incompatibleLicenses)
						{
                            if (line.Contains(license))
							{
                                Program.LogDebug("found incompatible license: " + license);
                                score -= 0.5F;
							}
						}
					}


                    // make sure score is within acceptable range
                    score = Math.Max(score, 0);
                    score = Math.Min(score, 1);

                }else{
                    // if there is no license at all, set the score to zero
                    this.score = 0;
                }    



            }
            catch (Octokit.AuthorizationException)
            {
                Program.LogError("Bad credentials. Check your access token.");
            }
            catch (Octokit.NotFoundException)
            {
                Program.LogError("Non existent repository");
            }
        }
    }
}