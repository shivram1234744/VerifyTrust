using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class JobDocumentDtl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        // Foreign Key to Job_Master
        [ForeignKey("Job_Master")]
        public int JobId { get; set; }

        [StringLength(255)]
        public string? DocumentName { get; set; }

        [NotMapped]
        public IFormFile? DocumentFile { get; set; } = null;

        // Navigation property
        [ForeignKey("JobId")]   
        public Job_Master JobMaster { get; set; }
    }
}
