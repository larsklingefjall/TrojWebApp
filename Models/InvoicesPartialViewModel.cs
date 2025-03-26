#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class InvoicesPartialViewModel
    {
        [Key]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }
     
        public required string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string? ReceiverName { get; set; }

        public int? Vat { get; set; }
        public double? Sum { get; set; }

        public bool Locked { get; set; }

        public DateTime Changed { get; set; }
        public required string ChangedBy { get; set; }
    }
}
