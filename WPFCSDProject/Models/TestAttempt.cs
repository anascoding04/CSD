namespace WPFCSDProject.Models
{
    public class TestAttempt
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public DateTime CompletedAt { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int PointsEarned { get; set; }
        public double Percentage { get; set; }
    }
}

