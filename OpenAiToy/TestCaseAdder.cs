using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraGPTGrader
{
    public class TestCaseAdder
    {
        private readonly JiraClient _jiraClient;
        private readonly GPTClient _gptClient;

        public TestCaseAdder(JiraClient jiraClient, GPTClient gptClient)
        {
            _jiraClient = jiraClient;
            _gptClient = gptClient;
        }

        // Method to add test cases and acceptance criteria
        public async Task AddTestCasesAndAcceptanceCriteria(IEnumerable<string> issueKeys)
        {
            foreach (var issueKey in issueKeys)
            {
                try
                {
                    // Get the issue type
                    string issueType = _jiraClient.GetJiraIssueType(issueKey);

                    // Skip if it's a bug/QA defect
                    if (issueType.Equals("Bug", StringComparison.OrdinalIgnoreCase) || issueType.Equals("QA-Defect", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Skipping test cases for {issueKey} as it's a {issueType}.");
                        continue; // Skip adding test cases and acceptance criteria
                    }

                    // Get the title and description
                    var (title, description) = _jiraClient.GetJiraStoryDetails(issueKey);

                    // Generate test cases and acceptance criteria
                    string generatedContent = await _gptClient.GenerateTestCasesAndAcceptanceCriteria(title, description);

                    // Append to the Jira story description
                    await _jiraClient.AppendToJiraStoryDescriptionAsync(issueKey, generatedContent);

                    Console.WriteLine($"Added test cases for {issueKey}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {issueKey}: {ex.Message}");
                }
            }
        }
    }
}
