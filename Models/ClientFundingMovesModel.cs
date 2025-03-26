using System.ComponentModel.DataAnnotations;
using System;

namespace TrojWebApp.Models
{
    public class ClientFundingMovesModel
    {
        [Key]
        public int Id { get; set; }

        public int FromId { get; set; }
        public int ToId { get; set; }
        public DateTime Changed { get; set; }
        public string ChangedBy { get; set; }
    }
}
