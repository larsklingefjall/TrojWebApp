using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class WorkingTimesModel
    {
        [Key]
        public int WorkingTimeId { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        [ForeignKey("TariffTypes")]
        public int TariffTypeId { get; set; }

        [ForeignKey("Employees")]
        public int EmployeeId { get; set; }

        public DateTime? WhenDate { get; set; }
        public double? TariffLevel { get; set; }
        public double? NumberOfHours { get; set; }
        public double? Cost { get; set; }
        public double? Sum { get; set; }
        public string? Comment { get; set; }
        public bool Billed { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
