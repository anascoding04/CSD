namespace WPFCSDProject.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public int Points { get; set; } = 30;
    }
}

