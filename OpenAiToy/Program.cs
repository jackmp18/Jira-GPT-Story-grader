using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraGPTGrader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Example usage
            string jiraBaseUrl = "https://freightpop.atlassian.net";
            List<string> issueKeys = new List<string> { "DA-1896", "DA-1897", "DA-1898" };
            string jiraUsername = "jiraapiuser@freightpop.com";
            string jiraApiToken = "ATATT3xFfGF0ju8nEW57JWBre8tn6nK5mmLyTWrqMBZNTfH2rX9EBm29R0J9uRO4v5n-jYbQ9hiYZR6BxOPjJFOYC6qfNfW5IjlHv1v9GY81tCrBpOFqFn3MusQ1-PMKG6_5RkDqeMj4Ku-T8gCAeYAZlTSRlltOnoWnCw8n1IKE-BVUiDRJ1Yc=0ED9CB68";
            string openAiApiKey = Environment.GetEnvironmentVariable("Chatgpt");
             
            var jiraClient = new JiraClient(jiraBaseUrl, jiraUsername, jiraApiToken);
            var gptClient = new GPTClient(openAiApiKey);
            var storyGrader = new StoryGrader(jiraClient, gptClient);

            // Grade the stories
            List<StoryFeedback> feedbacks = await storyGrader.GradeJiraStoriesAsync(issueKeys);

            // Export to Excel
            string filePath = "C:\\Projects\\OpenAiToy\\Logs for files changed\\StoryFeedback.xlsx";
            ExcelExporter.ExportToExcel(filePath, feedbacks);

            Console.WriteLine();
            Console.WriteLine($"Results exported to {filePath}");
            Console.ReadLine();
        }
    }
}
