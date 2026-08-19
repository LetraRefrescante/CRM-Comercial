using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class SectorRepository
    {
        public List<Sector> Listar(bool incluirInativos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Sectors.AsQueryable();
                if (!incluirInativos) query = query.Where(s => s.IsActive);
                return query.OrderBy(s => s.Name).ToList();
            }
        }
        public List<Sector> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Sectors.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarSectorId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Sectors.Where(s => s.Name == name);
                if (ignorarSectorId.HasValue) query = query.Where(s => s.SectorId != ignorarSectorId.Value);
                return query.Any();
            }
        }

        public int Criar(Sector sector)
        {
            using (var context = new CrmDbContext())
            {
                sector.CreatedDate = DateTime.UtcNow;
                sector.IsActive = true;
                context.Sectors.Add(sector);
                context.SaveChanges();
                return sector.SectorId;
            }
        }

        public void AlternarEstado(int sectorId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var sector = context.Sectors.Find(sectorId);
                if (sector == null) return;

                sector.IsActive = !sector.IsActive;
                sector.UpdatedDate = DateTime.UtcNow;
                sector.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}