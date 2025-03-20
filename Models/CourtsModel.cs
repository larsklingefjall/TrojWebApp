using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CourtsModel
    {
        [Key]
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string Country { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
