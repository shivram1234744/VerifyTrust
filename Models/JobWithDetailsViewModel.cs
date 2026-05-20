using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustPlus.Models
{
    public class JobWithDetailsViewModel
    {
        [Key]
        public int JobId { get; set; }
        public int? ClientId { get; set; }
        [NotMapped]
        public string? ClientName { get; set; }
        public int? CaseId { get; set; }
        public string EmployeeName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PostionApplied { get; set; }
        public string Department { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime? JobDate { get; set; }
        public string FinalReport { get; set; }
        public string FinalSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Job_Dtl list
        public List<JobDtlViewModel> JobDetails { get; set; } = new List<JobDtlViewModel>();
        public string AssignedUserName { get; set; }
    }


}
