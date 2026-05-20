namespace TrustPlus.Models
{
    public class DetailsViewModel
    {
        public int JobId { get; set; }
        public int? ClientId { get; set; }
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

        // Progress
        public int Progress { get; set; }

        // Job_Dtl + Client_Master
        public List<JobDtlViewModel> JobDetails { get; set; } = new List<JobDtlViewModel>();

        // Verification_Status
        public List<VerificationViewModel> Verifications { get; set; } = new List<VerificationViewModel>();
    }
}
