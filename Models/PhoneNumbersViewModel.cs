﻿#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrojWebApp.Models
{
    public class PhoneNumbersViewModel
    {
        [Key]
        public int PhoneNumberId { get; set; }

        [ForeignKey("Persons")]
        public int PersonId { get; set; }

        [ForeignKey("PhoneNumberTypes")]
        public int PhoneNumberTypeId { get; set; }

        public required string PhoneNumberType { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Changed { get; set; }
        public string? ChangedBy { get; set; }
    }
}
