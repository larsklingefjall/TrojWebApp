using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class MenuPagesViewModel
    {

        [Key]
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Id { get; set; }
    }
}
