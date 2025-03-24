#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CaseLogsViewModel
    {
        [Key]
        public int CaseLogId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        public string? WhenDate { get; set; }
        public string? Comment { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
