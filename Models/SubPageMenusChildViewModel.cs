using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class SubPageMenusChildViewModel
    {
        [Key]
        public int SubPageMenuId { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Title { get; set; }
        public string Tip { get; set; }
    }
}
