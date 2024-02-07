using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class InvoiceEconomyModel
    {
        [Key]
        public int InvoiceId { get; set; }
        public int CaseId { get; set; }
        public string InvoiceNumber { get; set; }
        public double Sum { get; set; }
        public int Vat { get; set; }
    }
}
