using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class TotalSumModel
    {
        [Key]
        public double? TotalSum { get; set; }
    }
}
