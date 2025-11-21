namespace WPFCSDProject.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int LecturerId { get; set; }
        public int QuestionCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}

