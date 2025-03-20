using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class TotalSumModel
    {
        [Key]
        public double? TotalSum { get; set; }
    }
}
