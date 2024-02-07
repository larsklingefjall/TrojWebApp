using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
