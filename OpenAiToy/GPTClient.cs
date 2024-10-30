using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace JiraGPTGrader
{
    public class GPTClient
    {
        private readonly ChatClient _chatClient;

        public GPTClient(string apiKey)
        {
            _chatClient = new ChatClient("gpt-4o-mini", new System.ClientModel.ApiKeyCredential(apiKey));
        }

        public async Task<StoryFeedback> GradeStoryWithGPT(string title, string storyDescription, string issueType)
        {
            try
            {
                List<ChatMessage> messages = new List<ChatMessage>
                {
                    new SystemChatMessage(@"You are a tool designed to help us create better agile stories and bugs by grading them on a 1-5 scale (1=VeryBad, 2=Bad, 3=Neutral, 4=Good, 5=VeryGood) and providing brief feedback. Please assess the stories based on the following criteria and provide your evaluation accordingly.

                        Grading Criteria:
                        1. **Clarity**: Is the story clear and easily understandable? Does it avoid ambiguity and provide a straightforward description of the requirements? Consider if the story uses precise language and avoids jargon that might confuse developers or stakeholders. Does the story explain the purpose of the feature or task? Jargon is ok for this orginization.

                        2. **Completeness**: Does the story include all necessary details for implementation? Check if it specifies all functional and non-functional requirements. Are acceptance criteria, solution steps, dependencies, and any relevant notes or backstories clearly provided? Does the story leave room for questions, or is everything included to ensure smooth execution?

                        3. **Testability**: Are there clear and actionable acceptance criteria that allow for effective testing of the story? Is it possible to validate the completion of the story through defined test cases? Ensure that the story allows QA or automated testing teams to verify whether the functionality meets the defined criteria.

                        4. **Feasibility and Scope**: Does the story have a well-defined scope that fits within a sprint? Is the story achievable given the resources and constraints of the development team? Ensure that the size and complexity of the task are reasonable and that it's clear which team members or areas are responsible for the work.

                        5. **User Impact**: Does the story clearly communicate the value or impact on the end user? Evaluate whether the story emphasizes how the change will benefit the users or improve the product. Is there a clear connection between the feature and user needs?

                        Please provide brief feedback for improvement!
                        ")

                };

                messages.Add(new UserChatMessage($"Grade this Jira \"{issueType}\" with title \"{title}\" and description: \"{storyDescription}\". Provide feedback on its clarity, completeness, and any missing details. Please format the response as a JSON object with the following structure and be sure to no include any extra symbols after }} such as ''': {{ \"Score\": <number>, \"Feedback\": \"<text>\" }}"));

                var result = await _chatClient.CompleteChatAsync(messages);
                string jsonResponse = result.Value.Content[0].Text;

                // Parse the JSON response
                var feedback = Newtonsoft.Json.JsonConvert.DeserializeObject<StoryFeedback>(jsonResponse);

                if (feedback == null)
                {
                    throw new Exception("Failed to parse GPT response into structured format.");
                }

                return feedback;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get GPT response: {ex.Message}");
            }
        }
        public async Task<string> GenerateTestCasesAndAcceptanceCriteria(string title, string storyDescription)
        {
            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"You are a tool designed to help us generate test cases and acceptance criteria based on agile stories."),
                new UserChatMessage($@"
                    Generate test cases and acceptance criteria for the Jira story with title ""{title}"" and description: ""{storyDescription}"".

                    Format the output in the following way:

                    Acceptance Criteria:
                    - [Criterion 1]
                    - [Criterion 2]
                    - [Criterion 3]
                    - ...

                    How to test:
                    - [Test Step 1]
                    - [Test Step 2]
                    - [Test Step 3]
                    - ...
                    ")
                };

            var result = await _chatClient.CompleteChatAsync(messages);
            return result.Value.Content[0].Text; // Return the generated content
        }

    }
}
