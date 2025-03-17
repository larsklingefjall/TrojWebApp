using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class MailAddressesModel
    {
        [Key]
        public int MailAddressId { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }

        public string MailAddress { get; set; }
        public string Comment { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
