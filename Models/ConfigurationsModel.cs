using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class ConfigurationsModel
    {
        [Key]
        public int ConfigurationId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
