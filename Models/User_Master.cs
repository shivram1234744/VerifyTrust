using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class User_Master
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int Role { get; set; } = 4;
        public string? LoginId { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedAt { get; set; }= DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
