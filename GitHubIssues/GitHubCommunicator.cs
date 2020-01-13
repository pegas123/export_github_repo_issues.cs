using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubIssues
{
    internal class GitHubCommunicator
    {
        string REPO_OWNER;
        string REPO_NAME;
        private bool? useSimpleAut;
        string GITHUB_USER;
        string GITHUB_PASSWORD;
        string TOKEN;

        #region constructors
        /// <summary>
        /// default constructor disabled
        /// </summary>
        private GitHubCommunicator() { }

        /// <summary>
        /// constructor for public repositories that do not require authentication
        /// </summary>
        /// <param name="repoOwner"></param>
        /// <param name="repoName"></param>
        internal GitHubCommunicator(string repoOwner, string repoName) 
        {
            REPO_OWNER = repoOwner;
            REPO_NAME = repoName;
        }

        /// <summary>
        /// constructor for token authentication
        /// </summary>
        /// <param name="repoOwner"></param>
        /// <param name="repoName"></param>
        /// <param name="token"></param>
        internal GitHubCommunicator(string repoOwner, string repoName,
            string token) 
        {
            useSimpleAut = false;
            REPO_OWNER = repoOwner;
            REPO_NAME = repoName;
            TOKEN = token;
        }

        /// <summary>
        /// constructor for user/pwd authentication - deprecated
        /// </summary>
        /// <param name="repoOwner"></param>
        /// <param name="repoName"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        internal GitHubCommunicator(string repoOwner, string repoName,
            string user, string pwd) 
        {
            useSimpleAut = true;
            REPO_OWNER = repoOwner;
            REPO_NAME = repoName;
            GITHUB_USER = user;
            GITHUB_PASSWORD = pwd;
        }
        #endregion

        #region parameters
        /// <summary>
        /// to control the maximum number of issues returned
        /// </summary>
        public int MaxIssues { get; set; } = 100;

        public ItemStateFilter StateFilter { get; set; } = ItemStateFilter.Open;
        public IssueFilter IssueRelationFilter { get; set; } = IssueFilter.All;

        #endregion

        internal Tuple<List<Issue>, List<Tuple<int,IssueComment>>> GetIssues(bool withComments = false)
        {
            GitHubClient client = GetClient();
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
                new RepositoryIssueRequest() { State = StateFilter, Filter = IssueRelationFilter },
                new ApiOptions() { PageSize = MaxIssues });
            List<Issue> issues = issuesForOctokit.Result.ToList();
            var comments = new List<Tuple<int,IssueComment>>();
            if(withComments)
            {
                foreach(var issue in issues)
                {
                    if(issue.Comments > 0)
                    {
                        var issueCmments = client.Issue.Comment.GetAllForIssue(REPO_OWNER, REPO_NAME, issue.Number);
                        foreach(var comment in issueCmments.Result)
                        {
                            comments.Add(new Tuple<int, IssueComment>(issue.Number, comment));
                        }
                    }
                }
            }
            return new Tuple<List<Issue>, List<Tuple<int, IssueComment>>>(issues,comments);
        }

        private GitHubClient GetClient()
        {
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

            // check if authentication is needed
            if (useSimpleAut.HasValue)
            {
                // Authenticated Access
                // If you want to access private repositories or perform actions on behalf of a user,
                // you need to pass credentials to the client.
                //
                // There are two options supported by the API - basic and OAuth authentication.
                //
                if (useSimpleAut.Value)
                {
                    //Uses basic authentication(Github username + password) to retrieve Issues
                    //from a repository that username has access to.Supports Github API v3.
                    var basicAuth = new Credentials(GITHUB_USER, GITHUB_PASSWORD);
                    client.Credentials = basicAuth;

                }
                else
                {
                    // It is strongly recommended to use the OAuth Flow for interactions on behalf of a user,
                    // as this gives two significant benefits:
                    //
                    // the application owner never needs to store a user's password
                    // the token can be revoked by the user at a later date
                    // When authenticated, you have 5000 requests per hour available.
                    // So this is the recommended approach for interacting with the API.
                    var tokenAuth = new Credentials(TOKEN);
                    client.Credentials = tokenAuth;
                }
            }
            return client; // >>>>>
        }
    }
}
