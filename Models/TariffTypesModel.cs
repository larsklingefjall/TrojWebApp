using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class TariffTypesModel
    {
        [Key]
        public int TariffTypeId { get; set; }
        public string TariffType { get; set; }
        public bool NoLevel { get; set; }
        public bool Invisible { get; set; }
        public string BackgroundColor { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
