using System;
using System.ComponentModel.DataAnnotations;

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
