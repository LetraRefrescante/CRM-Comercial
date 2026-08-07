using System.Data.Entity;
using CRM.Models.Entities.Seguranca;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.Documentos;
using CRM.Models.Entities.Leads;

namespace CRM.Data.Context
{
    public class CrmDbContext : DbContext
    {
        public CrmDbContext() : base("name=CrmConnectionString")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        // Catálogo
        public DbSet<Category> Categories { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<Product> Products { get; set; }

        // Documentos
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAccessLog> DocumentAccessLogs { get; set; }

        // Leads
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadStatusHistory> LeadStatusHistories { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<LossReason> LossReasons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<User>()
                .HasRequired(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasRequired(c => c.Country)
                .WithMany(co => co.Clients)
                .HasForeignKey(c => c.CountryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasOptional(c => c.Sector)
                .WithMany(s => s.Clients)
                .HasForeignKey(c => c.SectorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasRequired(c => c.AccountManager)
                .WithMany()
                .HasForeignKey(c => c.AccountManagerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasRequired(ct => ct.Client)
                .WithMany(c => c.Contacts)
                .HasForeignKey(ct => ct.ClientId)
                .WillCascadeOnDelete(false);

            // Catálogo
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.TaxRate)
                .WithMany()
                .HasForeignKey(p => p.TaxRateId)
                .WillCascadeOnDelete(false);

            // Documentos
            modelBuilder.Entity<Document>()
                .HasOptional(d => d.RelatedClient)
                .WithMany()
                .HasForeignKey(d => d.RelatedClientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DocumentAccessLog>()
                .HasRequired(l => l.Document)
                .WithMany()
                .HasForeignKey(l => l.DocumentId)
                .WillCascadeOnDelete(false);

            // Leads
            modelBuilder.Entity<Lead>()
                .HasRequired(l => l.LeadSource)
                .WithMany()
                .HasForeignKey(l => l.LeadSourceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasOptional(l => l.LossReason)
                .WithMany()
                .HasForeignKey(l => l.LossReasonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasRequired(l => l.Owner)
                .WithMany()
                .HasForeignKey(l => l.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasOptional(l => l.ConvertedClient)
                .WithMany()
                .HasForeignKey(l => l.ConvertedClientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lead>()
                .HasOptional(l => l.ConvertedContact)
                .WithMany()
                .HasForeignKey(l => l.ConvertedContactId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LeadStatusHistory>()
                .HasRequired(h => h.Lead)
                .WithMany()
                .HasForeignKey(h => h.LeadId)
                .WillCascadeOnDelete(false);
        }
    }
}