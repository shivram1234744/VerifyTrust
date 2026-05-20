namespace TrustPlus.Models
{
    public class VerificationViewModel
    {
        public int Id { get; set; }
        public int VarId { get; set; }
        public int ClientId { get; set; }
        public string Status { get; set; }
        public string Flag { get; set; }
        public string DocumentPath { get; set; }
        public string ExecutorNotes { get; set; }
    }
}
