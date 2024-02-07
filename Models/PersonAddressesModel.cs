using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class PersonAddressesModel
    {
        [Key]
        public int PersonAddressId { get; set; }

        [ForeignKey("PersonsModel")]
        public int PersonId { get; set; }
        public string CareOf { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string Country { get; set; }
        public bool Valid { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
