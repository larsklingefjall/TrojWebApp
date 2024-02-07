using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class SumOfModel
    {
        [Key]
        public double SumOf { get; set; }
    }
}
