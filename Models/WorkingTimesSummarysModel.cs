#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class WorkingTimesSummarysModel
    {
        [Key]
        public required string TariffType { get; set; }

        public double? UnitCost { get; set; }
        public double? SumCosts { get; set; }
        public double? SumHours { get; set; }
        public required string BackgroundColor { get; set; }     
    }
}
