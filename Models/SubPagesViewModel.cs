using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPagesViewModel
    {
        [Key]
        public int SubPageId { get; set; }

        [ForeignKey("Pages")]
        public int PageId { get; set; }
        public string PageTitle { get; set; }
        public string Parent { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Tip { get; set; }
        public int Position { get; set; }
        public string Parameter { get; set; }
        public bool IsVisible { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
