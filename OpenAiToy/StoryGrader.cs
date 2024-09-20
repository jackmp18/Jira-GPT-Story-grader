using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraGPTGrader
{
    public class StoryGrader
    {
        private readonly JiraClient _jiraClient;
        private readonly GPTClient _gptClient;

        public StoryGrader(JiraClient jiraClient, GPTClient gptClient)
        {
            _jiraClient = jiraClient;
            _gptClient = gptClient;
        }

        public async Task<List<StoryFeedback>> GradeJiraStoriesAsync(IEnumerable<string> issueKeys)
        {
            var results = new List<StoryFeedback>();

            foreach (var issueKey in issueKeys)
            {
                try
                {
                    // Get the Jira story title and description
                    var (title, description) = _jiraClient.GetJiraStoryDetails(issueKey);

                    if (string.IsNullOrEmpty(description))
                    {
                        Console.WriteLine($"No description found for issue {issueKey}.");
                        continue;
                    }

                    // Send the details to GPT for grading
                    StoryFeedback feedback = await _gptClient.GradeStoryWithGPT(title, description);

                    // Define the necessary parameters for updating Jira
                    string jiraBaseUrl = "https://freightpop.atlassian.net";
                    string username = "jackp@freightpop.com";
                    string apiToken = Environment.GetEnvironmentVariable("Jira");

                    // Update the Jira ticket with GPT rating and feedback
                    _jiraClient.UpdateJiraStoryWithFeedback(jiraBaseUrl, issueKey, username, apiToken, feedback.Score, feedback.Feedback);

                    // Collect the result
                    results.Add(new StoryFeedback
                    {
                        IssueKey = issueKey,
                        Score = feedback.Score,
                        Feedback = feedback.Feedback
                    });

                    // Output structured GPT response
                    Console.WriteLine($"Graded feedback for the story {issueKey}:\nScore: {feedback.Score}\nFeedback: {feedback.Feedback}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred for issue {issueKey}: {ex.Message}");
                }
            }

            return results;
        }
    }
}
