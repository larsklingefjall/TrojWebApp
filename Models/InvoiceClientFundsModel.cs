#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class InvoiceClientFundsModel
    {
        [Key]
        public int InvoiceClientFundId { get; set; }

        [ForeignKey("ClientFundings")]
        public int ClientFundId { get; set; }


        [ForeignKey("Invoices")]
        public int InvoiceId { get; set; }

        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
