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

        public async Task<StoryFeedback> GradeStoryWithGPT(string title, string storyDescription)
        {
            try
            {
                List<ChatMessage> messages = new List<ChatMessage>
                {
                    new SystemChatMessage(@"You are a tool designed to help us create better agile stories by grading them on a 1-5 scale (1=VeryBad, 2=Bad, 3=Neutral, 4=Good, 5=VeryGood) and providing brief feedback. Please assess the stories based on the following criteria and provide your evaluation accordingly.
                        Grading Criteria:
                        Clarity: Is the story clear and easily understandable? Does it avoid ambiguity and provide a straightforward description of the requirements?
                        Completeness: Does the story include all necessary details for implementation? Are acceptance criteria, solution steps, and any relevant notes or backstories provided?
                        Testability: Are there clear and actionable acceptance criteria that allow for effective testing of the story? Is it possible to validate the completion of the story?
                        ")
                };

                messages.Add(new UserChatMessage($"Grade this Jira story with title \"{title}\" and description: \"{storyDescription}\". Provide feedback on its clarity, completeness, and any missing details. Please format the response as a JSON object with the following structure: {{ \"Score\": <number>, \"Feedback\": \"<text>\" }}."));

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
    }
}
