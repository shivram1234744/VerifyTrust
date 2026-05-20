using Microsoft.EntityFrameworkCore;

namespace TrustPlus.Models
{
    public class DbManager : DbContext
    {
        public DbManager(DbContextOptions<DbManager> options)
            : base(options)
        {
        }

        public DbSet<LoginModel> User_Master1 { get; set; }
        public DbSet<User_Master> User_Master { get; set; }

        public DbSet<Mst_VerificationCheck> Verification_Master { get; set; }

        public DbSet<ClientDtl> Client_Dtl { get; set; }
        public DbSet<Job_Dtl> Job_Dtl { get; set; }
        public DbSet<Client_Master> Client_Master { get; set; }
        public DbSet<JobDocumentDtl> JobDocument_Dtl { get; set; }
        public DbSet<Job_Master> Job_Master { get; set; }
       
        public DbSet<AssignedJobViewModel> AssignedJobViewModels { get; set; }
        public DbSet<JobWithDetailsViewModel> jobWithDetailsViewModel{ get; set; }
        public DbSet<JobDtlViewModel> JobDtlViewModel { get; set; }

        public DbSet<Verification> Verification_Status { get; set; }

        public DbSet<EmployeeMst>EmployeeMst { get; set; }

     
        // =========================================
        // Trigger Configuration
        // =========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Verification>()
                .ToTable("tbl_verification", tb =>
                {
                    tb.HasTrigger("trg_UpdateClientStatus");
                });
        }

    }
}