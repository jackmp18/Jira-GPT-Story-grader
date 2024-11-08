static async Task Main(string[] args)
{
    try
    {
        // Load configuration from file
        Config config = Config.LoadConfig();

        // Use loaded configuration
        string jiraBaseUrl = config.JiraBaseUrl;
        string jiraUsername = config.JiraUsername;
        string jiraApiToken = config.JiraApiToken;
        string openAiApiKey = config.OpenAiApiKey;

        // Prompt user for the list of issue keys
        List<string> issueKeys = PromptForIssueKeys();

        var jiraClient = new JiraClient(jiraBaseUrl, jiraUsername, jiraApiToken);
        var gptClient = new GPTClient(openAiApiKey);
        var storyGrader = new StoryGrader(jiraClient, gptClient);
        var testCaseAdder = new TestCaseAdder(jiraClient, gptClient); // Instantiate new TestCaseAdder class

        // Prompt user for action (1 = grade, 2 = add test cases, 3 = both)
        int action = PromptForAction();

        // Perform action based on user's choice
        List<StoryFeedback> feedbacks = new List<StoryFeedback>();
        if (action == 1 || action == 3)
        {
            // Grade stories
            feedbacks = await storyGrader.GradeJiraStoriesAsync(issueKeys);
        }

        if (action == 2 || action == 3)
        {
            // Add test cases and acceptance criteria using the TestCaseAdder class
            await testCaseAdder.AddTestCasesAndAcceptanceCriteria(issueKeys);
        }

        // Export to Excel if grading was done
        if (feedbacks.Count > 0)
        {
            string filePath = $".\\StoryFeedback.xlsx";
            ExcelExporter.ExportToExcel(feedbacks);
            Console.WriteLine($"Results exported to {filePath}");
        }

        Console.ReadLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.ReadLine();
    }
}


        // Method to prompt the user for Jira issue keys
        static List<string> PromptForIssueKeys()
        {
            Console.WriteLine("Please enter Jira issue keys separated by commas (Max 20):");
            string input = Console.ReadLine();
            List<string> issueKeys = new List<string>(input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < issueKeys.Count; i++)
            {
                issueKeys[i] = issueKeys[i].Trim();
            }
            return issueKeys;
        }

        // Method to prompt the user for action
        static int PromptForAction()
        {
            Console.WriteLine("Choose an action: \n1. Grade tickets\n2. Add test cases and acceptance criteria\n3. Do both");
            int action = int.Parse(Console.ReadLine());
            return action;
        }
    }
}
