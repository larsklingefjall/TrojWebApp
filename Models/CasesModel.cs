using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CasesModel
    {
        [Key]
        public int CaseId { get; set; }

        [ForeignKey("CaseTypesModel")]
        public int CaseTypeId { get; set; }
        public string Title { get; set; }
        public DateTime CaseDate { get; set; }
        public string Responsible { get; set; }
        public bool Active { get; set; }
        public bool Secrecy { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Comment { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }

    }
}
