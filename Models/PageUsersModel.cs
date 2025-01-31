using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class PageUsersModel
    {
        [Key]
        public int PageUserId { get; set; }

        [ForeignKey("Pages")]
        public int PageId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
