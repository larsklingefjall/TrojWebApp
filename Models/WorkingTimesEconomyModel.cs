#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class WorkingTimesEconomyModel
    {
        [Key]
        public int CaseId { get; set; }
        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FinishedDate { get; set; }
        public double WorkingTimesSum { get; set; }
    }
}
