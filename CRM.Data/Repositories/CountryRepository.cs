using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class CountryRepository
    {
        public bool EhPortugal(int countryId)
        {
            using (var context = new CrmDbContext())
            {
                var country = context.Countries.Find(countryId);
                return country != null && country.IsoCode == "PT";
            }
        }
        public List<Country> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Countries.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
            }
        }
        public List<Country> Listar(bool incluirInativos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Countries.AsQueryable();
                if (!incluirInativos) query = query.Where(c => c.IsActive);
                return query.OrderBy(c => c.Name).ToList();
            }
        }
        public bool ExisteCodigo(string code, int? ignorarCountryId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Countries.Where(c => c.IsoCode == code);
                if (ignorarCountryId.HasValue) query = query.Where(c => c.CountryId != ignorarCountryId.Value);
                return query.Any();
            }
        }
        public int Criar(Country country)
        {
            using (var context = new CrmDbContext())
            {
                country.CreatedDate = DateTime.UtcNow;
                country.IsActive = true;
                context.Countries.Add(country);
                context.SaveChanges();
                return country.CountryId;
            }
        }

        public void AlternarEstado(int countryId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var country = context.Countries.Find(countryId);
                if (country == null) return;

                country.IsActive = !country.IsActive;
                country.UpdatedDate = DateTime.UtcNow;
                country.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}