using RestSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JiraGPTGrader
{
    public class JiraClient
    {
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _apiToken;

        public JiraClient(string baseUrl, string username, string apiToken)
        {
            _baseUrl = baseUrl;
            _username = username;
            _apiToken = apiToken;
        }

        public (string Title, string Description) GetJiraStoryDetails(string issueKey)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Get);

            // Set Jira authentication
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_apiToken}"));
            request.AddHeader("Authorization", $"Basic {encodedCredentials}");

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // Parse the JSON response
                JObject jiraIssue = JObject.Parse(response.Content);
                string title = jiraIssue["fields"]["summary"]?.ToString();
                string description = jiraIssue["fields"]["description"]?.ToString();
                return (title, description);
            }
            else
            {
                throw new Exception($"Failed to get Jira issue details: {response.ErrorMessage}");
            }
        }

        public void UpdateJiraStoryWithFeedback(string baseUrl, string issueKey, string username, string apiToken, int score, string feedback)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Put);

            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{apiToken}"));
            request.AddHeader("Authorization", $"Basic {encodedCredentials}");
            request.AddHeader("Content-Type", "application/json");

            // Create the payload to update the custom field
            var payload = new
            {
                fields = new
                {
                    customfield_10391 = $"Score: {score}\nFeedback: {feedback}"
                }
            };

            request.AddJsonBody(payload);
            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to update Jira issue: {response.ErrorMessage}");
            }
        }

        public async Task AppendToJiraStoryDescriptionAsync(string issueKey, string contentToAdd)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Put);

            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_apiToken}"));
            request.AddHeader("Authorization", $"Basic {encodedCredentials}");
            request.AddHeader("Content-Type", "application/json");

            // Fetch existing description
            var (title, description) = GetJiraStoryDetails(issueKey);

            // Append generated content to the bottom of the description
            string updatedDescription = description + $"\n\n*===ChatGPT Generated Test Cases & Acceptance Criteria===*\n{contentToAdd}";

            var payload = new
            {
                fields = new
                {
                    description = updatedDescription
                }
            };

            request.AddJsonBody(payload);

            // Use ExecuteAsync for asynchronous operation
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Failed to update Jira issue: {response.ErrorMessage}");
            }
        }

        public string GetJiraIssueType(string issueKey)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Get);

            // Set Jira authentication
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_apiToken}"));
            request.AddHeader("Authorization", $"Basic {encodedCredentials}");

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // Parse the JSON response to get issue type
                JObject jiraIssue = JObject.Parse(response.Content);
                string issueType = jiraIssue["fields"]["issuetype"]["name"]?.ToString(); // Adjust the path as needed
                return issueType;
            }
            else
            {
                throw new Exception($"Failed to get Jira issue details: {response.ErrorMessage}");
            }
        }

    }
}
