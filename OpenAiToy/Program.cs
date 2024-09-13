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
            string jiraUsername = "jackp@freightpop.com";
            string jiraApiToken = Environment.GetEnvironmentVariable("Jira");
            string openAiApiKey = Environment.GetEnvironmentVariable("Chatgpt");

            var jiraClient = new JiraClient(jiraBaseUrl, jiraUsername, jiraApiToken);
            var gptClient = new GPTClient(openAiApiKey);
            var storyGrader = new StoryGrader(jiraClient, gptClient);

            // Grade the stories
            List<StoryFeedback> feedbacks = await storyGrader.GradeJiraStoriesAsync(issueKeys);

            // Export to Excel
            string filePath = "C:\\Projects\\OpenAiToy\\Logs for files changed\\StoryFeedback.xlsx";
            ExcelExporter.ExportToExcel(filePath, feedbacks);

            Console.WriteLine($"Results exported to {filePath}");
            Console.ReadLine();
        }
    }
}
