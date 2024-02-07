using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class NumberOfModel
    {
        [Key]
        public int NumberOf { get; set; }
    }
}
