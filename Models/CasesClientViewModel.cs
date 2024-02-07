using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class CasesClientViewModel
    {
        [Key]
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
