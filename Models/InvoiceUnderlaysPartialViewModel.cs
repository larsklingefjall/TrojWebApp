using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceUnderlaysPartialViewModel
    {
        [Key]
        public int InvoiceUnderlayId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }
        public string? CaseType { get; set; }

        public string? UnderlayNumber { get; set; }
        public DateTime? UnderlayDate { get; set; }
        public string? ReceiverName { get; set; }

        public bool Locked { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
