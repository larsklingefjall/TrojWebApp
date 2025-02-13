using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class LoadTimesModel
    {
        [Key]
        public int LoadTimeId { get; set; } 
        public string Host { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int LoadTime { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
