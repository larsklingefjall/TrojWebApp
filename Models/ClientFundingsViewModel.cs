using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class ClientFundingsViewModel
    {
        [Key]
        public int ClientFundId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }
        public double? ClientSum { get; set; }
        public string? Comment { get; set; }
        public string? ClientFundDate { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
