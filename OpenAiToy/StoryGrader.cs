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
                    string username = "jiraapiuser@freightpop.com";
                    string apiToken = "ATATT3xFfGF0ju8nEW57JWBre8tn6nK5mmLyTWrqMBZNTfH2rX9EBm29R0J9uRO4v5n-jYbQ9hiYZR6BxOPjJFOYC6qfNfW5IjlHv1v9GY81tCrBpOFqFn3MusQ1-PMKG6_5RkDqeMj4Ku-T8gCAeYAZlTSRlltOnoWnCw8n1IKE-BVUiDRJ1Yc=0ED9CB68";

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
