using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class InvoiceAndCaseModel
    {
        [Key]
        public int InvoiceId { get; set; }
        public int CaseId { get; set; }
        public string? InvoiceNumber { get; set; }
        public double? Sum { get; set; }
        public int? Vat { get; set; }
    }
}
