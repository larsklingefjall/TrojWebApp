#nullable enable
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class EmployeesModel
    {
        [Key]
        public int EmployeeId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Initials { get; set; }
        public string? MailAddress { get; set; }
        public string? EmployeeTitle { get; set; }
        public string? SignatureLink { get; set; }
        public bool Represent { get; set; }
        public bool Active { get; set; }
        public bool ReadOnly { get; set; }
        public string? UserName3 { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
