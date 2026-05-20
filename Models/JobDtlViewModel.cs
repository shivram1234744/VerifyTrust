using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class JobDtlViewModel
    {
        [Key]
        public int JobDtlId { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public DateTime? AssignedDate { get; set; }
        public int? AssignedTo { get; set; }
        public string AssignedToName { get; set; }
    }
}
