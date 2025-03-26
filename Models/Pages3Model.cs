#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class Pages3Model
    {
        [Key]
        public int PageId { get; set; }
        public required string Title { get; set; }
        public required string Controller { get; set; }
        public required string Action { get; set; }
        public string? Tip { get; set; }
        public int Position { get; set; }
        public bool HasChild { get; set; }
        public DateTime Changed { get; set; }   
        public required string ChangedBy { get; set; }
    }
}
