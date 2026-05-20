using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class Job_Master
    {
        [Key]   //  Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobId { get; set; }

   
        public int CaseId { get; set; }

        public int? ClientId { get; set; }

        [StringLength(100)]
        public string? EmployeeName { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? PostionApplied { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Pending";


        [DataType(DataType.Date)]
        public DateTime? JobDate { get; set; } = DateTime.Now;

        public string? FinalReport { get; set; }

        public string? FinalSummary { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]

        [NotMapped]
        public IFormFile? FinalReportFile { get; set; }

        public ICollection<JobDocumentDtl> Documents { get; set; } = new List<JobDocumentDtl>();

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        //  Navigation property for documents
        [NotMapped]
        public List<IFormFile>? DocumentFiles { get; set; }

        [NotMapped]
        public string? ClientName { get; set; }


        //[ForeignKey("ClientId")]
        //public Client_Master Client_Master { get; set; }

    }
}
