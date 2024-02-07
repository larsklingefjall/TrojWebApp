using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class MaxMinDateModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
    }
}
