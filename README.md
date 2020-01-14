# export_repo_issues.cs
Export Issue list from GitHub repository

Simple/minimalistic C# Console Application to pull the list of Issues from private or public GitHub project repository.

The current varsion creates a CSV file with an issue and comment lines. Comments are included by default, can be excluded at Project.cs Line 43 by setting the parameter withComments: false.

Enables Issue filtration by 

 * StateFilter
 
        //     Items that are open.
        
        //     Items that are closed.
        
        //     All the items. (Default)

*  IssueFilter

        //     Issues assigned to the authenticated user.
        
        //     Issues created by the authenticated user.
        
        //     Issues mentioning the authenticated user.
        
        //     Issues the authenticated user is subscribed to for updates.
        
        //     All issues the authenticated user can see, regardless of participation or creation. (Default)

Uses https://github.com/octokit/octokit.net
