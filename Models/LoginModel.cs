namespace TrustPlus.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {

        [Key]   // Identity Primary Key
        public int UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }   // Nullable string

        [StringLength(15)]
        public string? MobileNumber { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(50)]
        public int? Role { get; set; }

        [StringLength(50)]
        public string? LoginId { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; }   // Nullable DateTime

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }   // Nullable DateTime
    }
}
