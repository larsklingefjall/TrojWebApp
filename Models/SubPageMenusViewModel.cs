using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPageMenusViewModel
    {
        [Key]
        public int SubPageMenuId { get; set; }

        [ForeignKey("SubPages3")]
        public int ParentPageId { get; set; }

        [ForeignKey("SubPages3")]
        public int ChildPageId { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }


        public string ChildController { get; set; }
        public string ChildAction { get; set; }
        public string ChildTitle { get; set; }

        public int? Position { get; set; }

        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
