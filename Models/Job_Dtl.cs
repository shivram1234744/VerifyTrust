using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrustPlus.Models
{
    public class Job_Dtl
    {
        [Key]   //  Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobDtlId { get; set; }          
        public int JobId { get; set; }             
        //public int? ClientId { get; set; }             
        public int VerificationId { get; set; }    

        public string VarificationName { get; set; }  

        public string? CompanyDocument { get; set; }   
        public string Status { get; set; }            
        public string? IssueType { get; set; }         
        public string? ExcuterRemark { get; set; }     
        public string? EvidanceDocument { get; set; }

        public DateTime? VarificationDate { get; set; } 
        public int? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;     
    }
}
