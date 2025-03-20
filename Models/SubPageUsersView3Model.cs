using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPageUsersView3Model
    {
        [Key]
        public int SubPageUserId { get; set; }

        [ForeignKey("SubPages3")]
        public int SubPageId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Position { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Initials { get; set; }

        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
