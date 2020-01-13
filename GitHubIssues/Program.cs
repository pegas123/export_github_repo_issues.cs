using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubIssues
{
    /// <summary>
    /// Exports Issues from a specified GITHUB repository
    /// to a CSV file
    /// or to Excel
    /// Using Octokit 
    /// </summary>
    /// <see cref="https://developer.github.com/v3/issues/"/>
    /// <see cref="https://github.com/octokit/octokit.net"/>
    /// <see cref="https://octokitnet.readthedocs.io/en/latest/issues/"/>
    /// <remarks>
    /// </remarks>
    class Program
    {
        const string GITHUB_USER = "pegas123";
        const string GITHUB_PASSWORD = "pzqw3QiSaLbDES";
        const string REPO_OWNER = "VestalEnterprises";
        const string REPO_NAME = "workforceconx";
        const int MAX_ISSUES = 100;
        static void Main(string[] args)
        {
            GitHubClient client = GetClient();

            List<Issue> issues = GetIssues(client, ItemStateFilter.All);


        }

        private static GitHubClient GetClient()
        {
            //Uses basic authentication(Github username + password) to retrieve Issues
            //from a repository that username has access to.Supports Github API v3.
            //Getting Started
            //The easiest way to get started with Octokit is to use a plain GitHubClient:
            //
            string CLIENT_TOKEN = $"BSP.PULL_ISSUES.{REPO_OWNER}.{REPO_NAME}";
            // But why do you need this ProductHeaderValue value?
            //
            // The API will reject you if you don't provide a User-Agent header. 
            // This is also to identify applications that are accessing the API and enable GitHub 
            // to contact the application author if there are problems. So pick a name that stands out!
            //
            GitHubClient client = new GitHubClient(new ProductHeaderValue(CLIENT_TOKEN));
            //        This will let you access unauthenticated GitHub APIs, but you will be subject
            //          to rate limiting.
            //
            // Authenticated Access
            // If you want to access private repositories or perform actions on behalf of a user,
            // you need to pass credentials to the client.
            //
            // There are two options supported by the API - basic and OAuth authentication.
            //
            var basicAuth = new Credentials(GITHUB_USER, GITHUB_PASSWORD);
            client.Credentials = basicAuth;

            //var tokenAuth = new Credentials("token"); // NOTE: not real token
            //        client.Credentials = tokenAuth;
            // It is strongly recommended to use the OAuth Flow for interactions on behalf of a user,
            // as this gives two significant benefits:
            //
            // the application owner never needs to store a user's password
            // the token can be revoked by the user at a later date
            // When authenticated, you have 5000 requests per hour available.
            // So this is the recommended approach for interacting with the API.
            return client;
        }

        private static List<Issue> GetIssues(GitHubClient client,
                    ItemStateFilter itemStateFilter = ItemStateFilter.Open,
                    IssueFilter issueFilter = IssueFilter.All)
        {
            // 
            // Working with Issues -- Get All
            // If you want to view all assigned, open issues against repositories you belong to 
            // (either you own them, or you belong to a team or organization), use this method:
            // 
            // var issues = await client.Issue.GetAllForCurrent();
            // If you want to skip organization repositories, you can instead use this rather verbose method:
            // 
            // var issues = await client.Issue.GetAllForOwnedAndMemberRepositories();
            // If you know the specific repository, just invoke that:
            // 
            var issuesForOctokit = client.Issue.GetAllForRepository(REPO_OWNER, REPO_NAME, 
                new RepositoryIssueRequest() { State = itemStateFilter, Filter  = issueFilter  },
                new ApiOptions() { PageSize = MAX_ISSUES });
            List<Issue> issues = issuesForOctokit.Result.ToList();
            return issues;
        }
    }
}
