using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace CopyGitHubTeam
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 5)
            {
                Console.WriteLine("The wrong arguments were passed. Pass the following arguments:");
                Console.WriteLine("GitHub-Org Team-to-Copy Target-GitHub-Org New-Team-Name GitHub-Login-Token");
                return;
            }



            var app = App(args[0], args[1], args[2], args[3], args[4]);
            app.Wait();
        }

        static async Task App(string org, string team, string org2, string team2, string token)
        {
            var client = new GitHubClient(new ProductHeaderValue("CopyGitHubTeam"));
            client.Credentials = new Credentials(token);
            var orgs = client.Organization;
            var teams = orgs.Team;

            var orgTeam = await GetTeam(client, org, team);
            var org2Team = await GetTeam(client, org2, team2);

            var orgTeamUsers = await teams.GetAllMembers(orgTeam.Id);

            foreach(var user in orgTeamUsers)
            {
                await teams.AddMembership(org2Team.Id, user.Login);
            }

        }

        static async Task<Team> GetTeam(GitHubClient client, string org, string team)
        {
            var orgs = client.Organization;
            var teams = orgs.Team;
            var orgTeams = await teams.GetAll(org);
            var matchingOrgTeams = from t in orgTeams
                                   where t.Name == team
                                   select t;

            if (matchingOrgTeams.Count() != 1)
            {
                Console.WriteLine($"Team not found: {team}");
                return null;
            }

            return matchingOrgTeams.Single();
        }
    }
}
