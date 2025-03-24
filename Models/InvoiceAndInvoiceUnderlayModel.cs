#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceAndInvoiceUnderlayModel
    {
        [Key]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }

        public string? InvoiceNumber { get; set; }
        public double? Sum { get; set; }
        public int? Vat { get; set; }
    }
}
