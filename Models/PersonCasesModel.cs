using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class PersonCasesModel
    {
        [Key]
        public int PersonCaseId { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }

        [ForeignKey("Cases")]
        public int CaseId { get; set; }

        [ForeignKey("PersonTypes")]
        public int PersonTypeId { get; set; }

        public bool Responsible { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
