using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CaseNumbersViewModel
    {
        [Key]
        public int CaseNumberId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        [ForeignKey("Courts")]
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public string CaseNumber { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
