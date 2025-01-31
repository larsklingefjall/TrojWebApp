using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class SubPageUsersModel
    {
        [Key]
        public int SubPageUserId { get; set; }
        public int SubPageId { get; set; }
        public int EmployeeId { get; set; }
    }
}
