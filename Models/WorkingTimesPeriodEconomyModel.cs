using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class WorkingTimesPeriodEconomyModel
    {
        [Key]
        public int CaseId { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
    }
}
