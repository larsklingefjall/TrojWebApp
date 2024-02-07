using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class SumOfWorkingTimesModel
    {
        [Key]
        public int CaseId { get; set; }
        public double SumOfSum { get; set; }

    }
}
