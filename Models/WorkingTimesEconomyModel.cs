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
#nullable enable
        public string? FinishedDate { get; set; }
#nullable disable
        public double? WorkingTimesSum { get; set; }
    }
}
