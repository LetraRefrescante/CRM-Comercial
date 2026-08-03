using System.Data.Entity;
using CRM.Models.Entities.Seguranca;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.ListasAuxiliares;

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

        public DbSet<Country> Countries { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }

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
        }
    }
}