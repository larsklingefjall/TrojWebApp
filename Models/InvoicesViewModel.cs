#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class InvoicesViewModel
    {
        [Key]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }
        public string? UnderlayNumber { get; set; }
        public int CaseId { get; set; }
        public required string CaseType { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }
        public required string? Initials { get; set; }
        
        public required string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? InvoicePlace { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public string? ReceiverName { get; set; }
        public string? ContactName { get; set; }
        public string? CareOf { get; set; }
        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalAddress { get; set; }
        public string? Country { get; set; }

        public string? Headline1 { get; set; }
        public string? Headline2 { get; set; }
        public string? Text1 { get; set; }

        public int? Vat { get; set; }
        public double? Sum { get; set; }
        public int? Division { get; set; }

        public bool Locked { get; set; }
        public bool HideClientFunding { get; set; }

        public DateTime Changed { get; set; }
        public required string ChangedBy { get; set; }
    }
}
