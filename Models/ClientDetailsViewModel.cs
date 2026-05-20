namespace TrustPlus.Models
{
    public class ClientDetailsViewModel
    {
        public Job_Master Job { get; set; }
        public string ClientName { get; set; }
        public List<JobDocumentDtl> Documents { get; set; }
    }
}
