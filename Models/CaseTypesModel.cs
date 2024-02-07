using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrojWebApp.Models
{
    public class CaseTypesModel
    {
        [Key]
        public int CaseTypeId { get; set; }
        public string CaseType { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
