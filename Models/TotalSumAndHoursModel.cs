using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class TotalSumAndHoursModel
    {
        [Key]
        public double TotalSum { get; set; }
        public double TotalHours { get; set; }
    }
}
