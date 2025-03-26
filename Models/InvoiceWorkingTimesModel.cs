#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class InvoiceWorkingTimesModel
    {
        [Key]
        public int InvoiceWorkingTimeId { get; set; }

        [ForeignKey("InvoiceUnderlays")]
        public int InvoiceUnderlayId { get; set; }

        [ForeignKey("WorkingTimes")]
        public int WorkingTimeId { get; set; }

        public double? UnitCost { get; set; }
        public double? NumberOfHours { get; set; }
        public double? Sum { get; set; }
        public DateTime Changed { get; set; }
        public required string ChangedBy { get; set; }
    }
}
