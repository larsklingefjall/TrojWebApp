using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class MenuPagesModel
    {
        [Key]
        public int MenuPageId { get; set; }

        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Id { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
