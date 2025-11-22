using System;
using System.Collections.Generic;
using System.Text;

namespace WPFCSDProject.Models
{
    /// <summary>
    /// Specifies the set of roles that a user can have within the system.
    /// </summary>
    public enum UserRole
    {
        Student,
        Lecturer,
        Admin
    }

    /// <summary>
    /// This is an abstract base class representing a generic user in the system.
    /// Lecturers, admins, and Students will inherit from this class.
    /// </summary>
    public abstract class User
    {
        // Primary Key from Database
        public int UserID { get; set; }

        // Login Credentials
        public string Username { get; set; } = string.Empty;

        // Security: Stored as a hash, never plain text
        public string PasswordHash { get; set; } = string.Empty;

        // Personal Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Role: Matches the Database ENUM ('Student', 'Lecturer', 'Admin')
        public UserRole Role { get; set; }

        // Account Status Flags
        public bool IsApproved { get; set; } // Pending approval logic
        public bool IsEnabled { get; set; }  // Soft delete / Ban logic

        // Auditing
        // nullable (?) because they might not have logged in yet.
        public DateTime? LastLoginTime { get; set; }

        // --- Helper Properties (Logic, not Data) ---

        // Encapsulation: Combines names for UI display automatically.
        public string FullName => $"{FirstName} {LastName}";

        // UI-friendly date string (e.g., "12:30 - 20/10/25" or "Never")
        public string LastLoginDisplay => LastLoginTime.HasValue
            ? LastLoginTime.Value.ToString("HH:mm:ss - dd/MM/yy")
            : "Never";
    }
}
