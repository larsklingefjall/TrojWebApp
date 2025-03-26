#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class SubPageMenusChildViewModel
    {
        [Key]
        public int SubPageMenuId { get; set; }

        public required string Controller { get; set; }
        public required string Action { get; set; }
        public required string Title { get; set; }
        public string? Tip { get; set; }
    }
}
