namespace TrustPlus.Models
{
    public class Register_Client
    {
        public string WebAddress { get; set; }

        public string CUserId { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string ContactPersion { get; set; }
        public DateTime? AgreementDate { get; set; } = DateTime.Now;
        public DateTime? RenewalDate { get; set; } = DateTime.Now;

    }
}
