using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
