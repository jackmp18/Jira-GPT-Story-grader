AI-Powered Jira Story Grader and Test Case/Requirements Generator
Key Benefits
Cost Savings: Reduces costs by lowering bugs and minimizing post-release fixes.
Productivity Boost: Automates story refinement, decreasing workload for Product Owners (POs) and developers.
Customer Retention: Enhances product quality, promoting customer loyalty and satisfaction.
How It Works
This tool integrates seamlessly with Jira’s API to automate and streamline the creation and refinement of Jira stories. Here’s an overview of the core functionalities:

Automatic Story Grading (1-5 Scale)
The AI evaluates each story’s clarity, completeness, and relevance on a 1-5 scale to quickly highlight stories that may need further refinement. Stories that score lower can be prioritized for improvement.

Grading Criteria:
Clarity: Ensures the story is easy to understand, avoiding ambiguous language.
Completeness: Confirms that essential details (who, what, when, why) are included.
Relevance: Verifies alignment with project goals and deliverables.
The AI generates feedback based on these criteria to guide POs in enhancing the story’s clarity and completeness. The grade and feedback are then updated in custom "AI Feedback" fields within Jira, offering an easily accessible overview for POs and developers.

Automated Test Case and Acceptance Criteria Generation
For each user story, the tool automatically generates test cases and acceptance criteria (AC), populating a dedicated "Test Cases/AC" field in Jira. This ensures that every story has well-defined requirements, enhancing developer understanding and reducing misinterpretation risks.

Example Acceptance Criteria for a User Story (e.g., Login Feature):

"User is redirected to the dashboard upon successful login."
"Error message displays for incorrect password entries."
Additionally, the AI suggests edge cases to help the team proactively identify potential issues during the development process.

Enhanced Story Detail for Bug Reduction and Customer Retention
By adding detailed acceptance criteria and structured requirements, the tool reduces misunderstandings and minimizes the likelihood of bugs reaching production. This leads to a higher-quality product, improved customer satisfaction, and increased retention rates. Furthermore, the tool automatically updates the Jira description field to reflect these enhanced details, ensuring alignment between developers and POs.

Estimated Financial Impact
Bug Reduction: Fewer bugs lead to cost savings on post-release fixes and improve customer experience.
Productivity Gains: Automated grading and test case generation save hours of manual work, reducing labor costs and allowing POs to focus on higher-priority tasks.
Customer Retention: Improved product quality strengthens customer loyalty, protecting revenue streams and lowering churn rates.

# Configuration
Before running the project, you need to create a configuration file named appsettings.json in the root directory of the project. This file stores sensitive information such as Jira and OpenAI API credentials.

Example appsettings.json
json
Copy code
{
  "JiraBaseUrl": "https://your-jira-instance.atlassian.net",
  "JiraUsername": "your-jira-username",
  "JiraApiToken": "your-jira-api-token",
  "OpenAiApiKey": "your-openai-api-key"
}
Security Note
Make sure to add appsettings.json to your .gitignore file to prevent it from being included in version control:

appsettings.json
Setting Up Credentials
JiraBaseUrl: Your Jira instance base URL (e.g., https://yourcompany.atlassian.net).
JiraUsername: Your Jira username used for API authentication.
JiraApiToken: A Jira API token generated from your Jira account.
OpenAiApiKey: Your OpenAI API key.


📂 OpenAiTool
├── 📄 ExcelExporter.cs       # Handles exporting Jira story data and feedback to Excel files
├── 📄 GPTClient.cs           # Manages interactions with the GPT API for story grading and feedback
├── 📄 JiraClient.cs          # Provides methods to interface with the Jira API
├── 📄 packages.config        # Configuration for project dependencies
├── 📄 Program.cs             # Main entry point for executing the application
├── 📄 Readme.txt             # General documentation and usage information
├── 📄 StoryFeedback.cs       # Handles generation and formatting of story feedback
├── 📄 StoryGrader.cs         # Implements grading logic for evaluating Jira stories
└── 📄 TestCaseAdder.cs       # Adds test cases and acceptance criteria to Jira stories
