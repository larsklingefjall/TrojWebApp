#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class TariffLevelsModel
    {
        [Key]
        public int TariffLevelId { get; set; }

        [ForeignKey("TariffTypes")]
        public int TariffTypeId { get; set; }
        public double TariffLevel { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool Valid { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
