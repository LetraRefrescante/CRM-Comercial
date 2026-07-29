using System.Data.Entity;
using CRM.Models.Entities;

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Desativa a convenção de nomes no plural automático
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            // Chave composta: RolePermission (RoleId + PermissionId)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Evita cascade delete automático em FKs opcionais (evita ciclos)
            modelBuilder.Entity<User>()
                .HasOptional(u => u.Role)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}