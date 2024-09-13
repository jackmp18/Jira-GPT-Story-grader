namespace JiraGPTGrader
{
    public class StoryFeedback
    {
        public string IssueKey { get; set; }  // Ensure this property is added
        public int Score { get; set; }
        public string Feedback { get; set; }
    }
}
