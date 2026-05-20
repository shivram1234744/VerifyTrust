using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class Client_Master
    {
        [Key]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string? WebAddress { get; set; }
        
        public string CUserId { get; set; }
        public string? MobileNumber { get; set; }
        public string CPassword { get; set; }
        public string? Address { get; set; }
        public string ContactPerson { get; set; }
        public DateTime? AgreementDate { get; set; } = DateTime.Now;
        public DateTime? RenewalDate { get; set; } = DateTime.Now;
   
    }
}
