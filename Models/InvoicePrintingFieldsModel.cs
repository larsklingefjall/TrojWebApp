#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoicePrintingFieldsModel
    {
        [Key]
        public int InvoicePrintingField { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }


        public string? FieldName { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
