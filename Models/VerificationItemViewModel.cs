namespace TrustPlus.Models
{
    public class VerificationItemViewModel
    {
        public int Id { get; set; }               // Maps to JobDtlId (Primary Key)
        public int VarId { get; set; }            // Maps to VerificationId
        public string Name { get; set; }          // Maps to VarificationName
        public string Status { get; set; }        // Maps to Status
        public string Flag { get; set; }          // Maps to IssueType
        public string ExecutorNotes { get; set; } // Maps to ExcuterRemark

        public string DocumentPath { get; set; }  // Stores old/current file string (EvidanceDocument)
        public IFormFile EvidenceFile { get; set; } // Catches incoming uploaded raw files
    }
}