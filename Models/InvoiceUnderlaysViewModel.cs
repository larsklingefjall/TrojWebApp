using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceUnderlaysViewModel
    {
        [Key]
        public int InvoiceUnderlayId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }
        public string CaseType { get; set; }


        [ForeignKey("Persons")]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [ForeignKey("PersonAddresses")]
        public int PersonAddressId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }
        public string EmployeeTitle { get; set; }
        public string Initials { get; set; }


        public string? Title { get; set; }
        public string UnderlayNumber { get; set; }
        public string SignatureLink { get; set; }
        public DateTime UnderlayDate { get; set; }
        public string UnderlayPlace { get; set; }
        public string ReceiverName { get; set; }
        public string? CareOf { get; set; }
        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string? Country { get; set; }
        public string Headline1 { get; set; }
        public string Headline2 { get; set; }
        public string? WorkingReport { get; set; }
        public int Vat { get; set; }
        public double? Sum { get; set; }
        public bool Locked { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
