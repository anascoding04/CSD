using System;
using System.Collections.Generic;
using System.Text;

namespace WPFCSDProject.Models
{
    /// <summary>
    /// This class inherits from user and features properties that are
    /// only relevant for students
    /// </summary>
    public class Student : User
    {
        // Maps to 'studentNumber' in the database (unique)
        public string StudentNumber { get; set; } = string.Empty;

        // Maps to 'courseTitle' in the database
        public string CourseTitle { get; set; } = string.Empty;
    }
}
