using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class TotalSumAndHoursModel
    {
        [Key]
        public double TotalSum { get; set; }
        public double TotalHours { get; set; }
    }
}
