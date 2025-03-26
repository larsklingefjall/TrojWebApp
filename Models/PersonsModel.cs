#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace TrojWebApp.Models
{
    public class PersonsModel
    {
        [Key]
        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? PersonNumber { get; set; }
        public string? MailAddress { get; set; }
        public bool Active { get; set; }
        public bool NeedInterpreter { get; set; }
        public DateTime Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
