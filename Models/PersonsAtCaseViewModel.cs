using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class PersonsAtCaseViewModel
    {
        [Key]
        public int PersonCaseId { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [ForeignKey("Cases")]
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public DateTime CaseDate { get; set; }
        public bool Active { get; set; }


        [ForeignKey("PersonTypes")]
        public int PersonTypeId { get; set; }
        public string PersonType { get; set; }

        public bool Responsible { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
