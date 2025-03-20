using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPagesView3Model
    {
        [Key]
        public int SubPageId { get; set; }

        [ForeignKey("Pages3")]
        public int PageId { get; set; }
        public string PageTitle { get; set; }
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string? Tip { get; set; }
        public int Position { get; set; }
        public bool IsVisible { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
