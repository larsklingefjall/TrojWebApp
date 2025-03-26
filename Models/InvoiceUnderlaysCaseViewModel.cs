#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceUnderlaysCaseViewModel
    {
        [Key]
        public int InvoiceUnderlayId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        public DateTime? UnderlayDate { get; set; }
  
        public required string Responsible { get; set; }
        public required string CaseType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
