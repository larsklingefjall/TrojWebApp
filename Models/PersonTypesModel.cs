using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class PersonTypesModel
    {
        [Key]
        public int PersonTypeId { get; set; }
        public string PersonType { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
