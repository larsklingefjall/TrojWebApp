using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrojWebApp.Models
{
    public class PhoneNumberTypesModel
    {
        [Key]
        public int PhoneNumberTypeId { get; set; }
        public string PhoneNumberType { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
