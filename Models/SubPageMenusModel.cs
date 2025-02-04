using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPageMenusModel
    {
        [Key]
        public int SubPageMenuId { get; set; }

        [ForeignKey("SubPages")]
        public int ParentPageId { get; set; }

        [ForeignKey("SubPages")]
        public int ChildPageId { get; set; }

        public int Position { get; set; }

        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
