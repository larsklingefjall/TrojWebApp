using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class UnderlayWorkingTimeSummaryModel
    {
        [Key]
        public int TariffTypeId { get; set; }
        public double TariffLevel { get; set; }
        public double UnitCost { get; set; }
        public double SumCosts { get; set; }
        public double SumHours { get; set; }
    }
}
