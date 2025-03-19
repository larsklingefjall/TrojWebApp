using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class TotalSumIdModel
    {
        [Key]
        public int Id { get; set; }
        public double TotalSum { get; set; }
    }
}
