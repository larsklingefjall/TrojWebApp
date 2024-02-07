using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class ActiveTariffTypesModel
    {
        [Key]
        public int TariffTypeId { get; set; }
        public string TariffType { get; set; }
        public double TariffLevel { get; set; }
    }
}
