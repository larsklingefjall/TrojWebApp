using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class WorkingTimesViewModel
    {
        [Key]
        public int WorkingTimeId { get; set; }
        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int CaseId { get; set; }
#nullable enable
        public string? Title { get; set; }
#nullable disable
        public string? CaseType { get; set; }
        public int TariffTypeId { get; set; }
        public string TariffType { get; set; }
        public string? BackgroundColor { get; set; }
        public double TariffLevel { get; set; }
        public bool NoLevel { get; set; }
        public int EmployeeId { get; set; }
        public string? Initials { get; set; }
        public string? WhenDate { get; set; }
        public double? NumberOfHours { get; set; }
        public double? Cost { get; set; }
        public double? Sum { get; set; }
        public string? Comment { get; set; }
        public bool Billed { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
