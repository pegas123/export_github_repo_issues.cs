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
        static void Main(string[] args)
        {
            string repoOwner = Properties.Settings.Default.RepoOwner;
            string repoName = Properties.Settings.Default.RepoName;
            var communicator =
                // public repository does not require authentication
                  Properties.Settings.Default.PublicRepository
                    ? new GitHubCommunicator(repoOwner, repoName)
                // token authentication
                : Properties.Settings.Default.UseTokenAuthentication
                    ? new GitHubCommunicator(repoOwner, repoName,
                            Properties.Settings.Default.Token)
                // authenticate using user name and password
                    : new GitHubCommunicator(repoOwner, repoName,
                            Properties.Settings.Default.GitUserName,
                            Properties.Settings.Default.GitPwd);

            if(Properties.Settings.Default.MaxIssues > 0)
            {
                communicator.MaxIssues = Properties.Settings.Default.MaxIssues;
            }

            List<Issue> issues = communicator.GetIssues( ItemStateFilter.All);


        }


    }
}
