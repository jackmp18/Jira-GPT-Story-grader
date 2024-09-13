using RestSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

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
                throw new Exception($"Failed to fetch Jira issue: {response.ErrorMessage}");
            }
        }

        public void UpdateJiraStoryWithFeedback(string jiraBaseUrl, string issueKey, string username, string apiToken, int gptRating, string feedback)
        {
            var client = new RestClient(jiraBaseUrl);
            var request = new RestRequest($"/rest/api/2/issue/{issueKey}", Method.Put);

            // Set Jira authentication
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{apiToken}"));
            request.AddHeader("Authorization", $"Basic {encodedCredentials}");
            request.AddHeader("Content-Type", "application/json");

            // Prepare the payload to update the fields
            var payload = new
            {
                fields = new
                {
                    customfield_10390 = new
                    {
                        value = gptRating.ToString()  // Assuming customfield_10390 expects a value
                    }
                }
            };

            request.AddJsonBody(payload);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Failed to update Jira issue: {response.ErrorMessage}");
                Console.WriteLine($"Response Content: {response.Content}"); // Log detailed response
                throw new Exception($"Failed to update Jira issue: {response.ErrorMessage}");
            }
            else
            {
                Console.WriteLine("Jira story updated with GPT rating and feedback successfully.");
            }
        }
    }
}
