using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class AssignedJobViewModel
    {
        // Job_Master fields
        [Key]
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public string CaseId { get; set; }
        public string EmployeeName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PostionApplied { get; set; }
        public string Department { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime JobDate { get; set; }
        public string FinalReport { get; set; }
        public string FinalSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Job_Dtl fields
        public int? AssignedTo { get; set; }
      

        // Client_Master field (AssignedTo ka name)
        public string AssignedToName { get; set; }

        // Extra property for progress calculation
        public int Progress { get; set; }
    }
}
