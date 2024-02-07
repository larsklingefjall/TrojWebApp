using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class InvoiceUnderlaySummarysModel
    {
        [Key]
        public string TariffType { get; set; }

        public double? UnitCost { get; set; }
        public double SumCosts { get; set; }
        public double SumHours { get; set; }
        public string BackgroundColor { get; set; }     
    }
}
