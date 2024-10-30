namespace JiraGPTGrader
{
    public class StoryFeedback
    {
        public string IssueKey { get; set; }  // Ensure this property is added
        public int Score { get; set; }
        public string Feedback { get; set; }
        public string TestCasesAndAcceptanceCriteria { get; set; }  // New property for test cases and acceptance criteria
    }
}
