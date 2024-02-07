using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CaseLogsModel
    {
        [Key]
        public int CaseLogId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        public DateTime WhenDate { get; set; }
        public string Comment { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
