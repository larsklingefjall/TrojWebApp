using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class CaseTypesModel
    {
        [Key]
        public int CaseTypeId { get; set; }
        public string? CaseType { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
