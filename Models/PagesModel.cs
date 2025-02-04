using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class PagesModel
    {
        [Key]
        public int PageId { get; set; }
        public string Title { get; set; }
        public string Controller { get; set; }
        public string FileName { get; set; }
        public string Tip { get; set; }
        public string Link { get; set; }
        public int Position { get; set; }
        public string Hidden { get; set; }
        public bool HasChild { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
