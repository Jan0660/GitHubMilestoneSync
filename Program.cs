using System;
using Console = Log73.Console;
using System.IO;
using System.Linq;
using System.Text.Json;
using Log73;
using Log73.ExtensionMethod;
using Log73.Extensions;
using Octokit;

// configure logging
Console.Configure.UseNewtonsoftJson();
Console.Options.UseAnsi = false;
Console.Options.LogLevel = LogLevel.Debug;

var github = new GitHubClient(new ProductHeaderValue("GitHubMilestoneSync", "v1.0"))
{
    Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"))
};

foreach (var config in JsonSerializer.Deserialize<Config[]>(
             await File.ReadAllTextAsync(Environment.GetEnvironmentVariable("CONFIG_FILE") ?? "config.json"),
             new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             })!)
{
    foreach (var repoName in config.Repositories)
    {
        var repo = await github.Repository.Get(repoName.Split('/')[0], repoName.Split('/')[1]);
        $"Milestones for {repoName}".Dump();
        var milestones = await github.Issue.Milestone.GetAllForRepository(repo.Id, new MilestoneRequest()
        {
            // Only gives us Open by default
            State = ItemStateFilter.All
        });
        foreach (var mile in config.Milestones)
        {
            var match = milestones.FirstOrDefault(m => m.Title == mile.Title);
            if (match != null)
            {
                "Has".DumpDebug();
                if (match.State.StringValue != mile.State ||
                    match.Description != mile.Description)
                {
                    "Differs, updating.".Dump();
                    await github.Issue.Milestone.Update(repo.Id, match.Number, new MilestoneUpdate()
                    {
                        Description = mile.Description,
                        State = StateFromString(mile.State)
                    });
                }
            }
            else
            {
                "Doesn't have, creating.".DumpDebug();
                await github.Issue.Milestone.Create(repo.Id, new NewMilestone(mile.Title)
                {
                    Description = mile.Description,
                    State = StateFromString(mile.State)
                });
            }
        }

        $"Labels for {repoName}".Dump();
        var labels = await github.Issue.Labels.GetAllForRepository(repo.Id);
        foreach (var label in config.Labels)
        {
            var match = labels.FirstOrDefault(l => l.Name == label.Name);
            if (match != null)
            {
                "Has".DumpDebug();
                if (match.Color != label.Color ||
                    match.Description != label.Description)
                {
                    "Differs, updating.".DumpDebug();
                    await github.Issue.Labels.Update(repo.Id, label.Name, new LabelUpdate(label.Name, label.Color)
                    {
                        Description = label.Description
                    });
                }
            }
            else
            {
                "Doesn't have, creating.".DumpDebug();
                await github.Issue.Labels.Create(repo.Id, new NewLabel(label.Name, label.Color)
                {
                    Description = label.Description
                });
            }
        }
    }
}

"Finished.".Dump();
var rateLimit = github.GetLastApiInfo().RateLimit;
$"Remaining {rateLimit.Remaining} of {rateLimit.Limit} rate limit.".Dump();

static ItemState StateFromString(string str)
    => str.ToLower() switch
    {
        "open" => ItemState.Open,
        "closed" => ItemState.Closed,
        _ => throw new Exception("Invalid state!")
    };

class Config
{
    public string[] Repositories { get; set; }
    public Milestone[] Milestones { get; set; }
    public Label[] Labels { get; set; }
}

class Milestone
{
    public string Title { get; set; }
    public string State { get; set; }
    public string Description { get; set; }
}

class Label
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
}