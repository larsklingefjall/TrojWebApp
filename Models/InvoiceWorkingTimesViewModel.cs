using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceWorkingTimesViewModel
    {
        [Key]
        public int InvoiceWorkingTimeId { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }

        [ForeignKey("WorkingTimes")]
        public int WorkingTimeId { get; set; }

        public DateTime WhenDate { get; set; }
        public bool Billed { get; set; }
        public string TariffType { get; set; }
        public string BackgroundColor { get; set; }
        public string Comment { get; set; }
        public double UnitCost { get; set; }
        public double NumberOfHours { get; set; }
        public double Sum { get; set; }
        public string Initials { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
