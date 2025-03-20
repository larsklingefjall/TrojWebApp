using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class PageUsersView3Model
    {
        [Key]
        public int PageUserId { get; set; }

        [ForeignKey("Pages3")]
        public int PageId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public string Title { get; set; }
        public int Position { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Initials { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }

    }
}
