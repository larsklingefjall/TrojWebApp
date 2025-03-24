using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class ActiveCasesWorkingTimesViewModel
    {
        [Key]
        public int CaseId { get; set; }
        public string CaseType { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Changed { get; set; }       

    }
}
