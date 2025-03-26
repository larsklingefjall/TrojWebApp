using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPageUsers3Model
    {
        [Key]
        public int SubPageUserId { get; set; }

        [ForeignKey("Pages3")]
        public int SubPageId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
