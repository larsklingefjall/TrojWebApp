#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class InvoiceSummarysViewModel
    {
        [Key]
        public int InvoiceSummaryId { get; set; }

        [ForeignKey("Invoices")]
        public int InvoiceId { get; set; }

        [ForeignKey("TariffTypes")]
        public int TariffTypeId { get; set; }
        public required string TariffType { get; set; }
        public required string BackgroundColor { get; set; }


        public double? UnitCost { get; set; }
        public double? UnitCounts { get; set; }
        public double? Sum { get; set; }

        public DateTime Changed { get; set; }
        public required string ChangedBy { get; set; }
    }
}
