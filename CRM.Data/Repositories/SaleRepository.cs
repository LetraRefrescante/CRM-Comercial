using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Vendas;

namespace CRM.Data.Repositories
{
    public class SaleRepository
    {
        public Sale GetById(int saleId)
        {
            using (var context = new CrmDbContext())
            {
                var sale = context.Sales
                    .Include(s => s.Client)
                    .Include(s => s.Client.AccountManager)
                    .Include(s => s.Proposal)
                    .Include(s => s.Owner)
                    .Include(s => s.Lines.Select(l => l.Product))
                    .Include(s => s.Lines.Select(l => l.TaxRate))
                    .Include(s => s.Payments)
                    .Where(s => s.SaleId == saleId && !s.IsDeleted)
                    .SingleOrDefault();

                if (sale != null)
                {
                    sale.Lines = sale.Lines.Where(l => !l.IsDeleted).ToList();
                    sale.Payments = sale.Payments.Where(p => !p.IsDeleted).ToList();
                }

                return sale;
            }
        }

        public bool ExisteVendaParaProposta(int proposalId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Sales.Any(s => s.ProposalId == proposalId && !s.IsDeleted);
            }
        }

        public List<Sale> Listar(
            string pesquisa,
            string status,
            int? clientId,
            int? ownerId,
            DateTime? dataInicio,
            DateTime? dataFim,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos,
            string sortColumn = "SaleDate",
            bool sortAscending = false)
        {
            using (var context = new CrmDbContext())
            {
                var query = ConstruirQuery(context, pesquisa, status, clientId, ownerId, dataInicio, dataFim);

                totalRegistos = query.Count();

                return AplicarOrdenacao(query, sortColumn, sortAscending)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        public List<Sale> ListarParaSelecao()
        {
            using (var context = new CrmDbContext())
            {
                return context.Sales.Where(s => !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedDate).ToList();
            }
        }

        private IQueryable<Sale> ConstruirQuery(
            CrmDbContext context,
            string pesquisa,
            string status,
            int? clientId,
            int? ownerId,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var query = context.Sales
                .Include(s => s.Client)
                .Include(s => s.Owner)
                .Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(s =>
                    s.SaleNumber.Contains(pesquisa) ||
                    s.Client.TradeName.Contains(pesquisa));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s => s.Status == status);
            }

            if (clientId.HasValue)
            {
                query = query.Where(s => s.ClientId == clientId.Value);
            }

            if (ownerId.HasValue)
            {
                query = query.Where(s => s.OwnerId == ownerId.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(s => s.SaleDate >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(s => s.SaleDate <= dataFim.Value.Date);
            }

            return query;
        }

        private IQueryable<Sale> AplicarOrdenacao(IQueryable<Sale> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "SaleNumber":
                    return sortAscending ? query.OrderBy(s => s.SaleNumber) : query.OrderByDescending(s => s.SaleNumber);
                case "Client":
                    return sortAscending ? query.OrderBy(s => s.Client.TradeName) : query.OrderByDescending(s => s.Client.TradeName);
                case "Status":
                    return sortAscending ? query.OrderBy(s => s.Status) : query.OrderByDescending(s => s.Status);
                case "Total":
                    return sortAscending ? query.OrderBy(s => s.Total) : query.OrderByDescending(s => s.Total);
                case "Owner":
                    return sortAscending ? query.OrderBy(s => s.Owner.Name) : query.OrderByDescending(s => s.Owner.Name);
                case "SaleDate":
                default:
                    return sortAscending ? query.OrderBy(s => s.SaleDate) : query.OrderByDescending(s => s.SaleDate);
            }
        }

        public Sale Criar(Sale sale)
        {
            const int maxTentativas = 3;

            using (var context = new CrmDbContext())
            {
                for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
                {
                    using (var transacao = context.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {
                            sale.SaleNumber = GerarProximoNumero(context);
                            context.Sales.Add(sale);
                            context.SaveChanges();
                            transacao.Commit();
                            return sale;
                        }
                        catch (DbUpdateException) when (tentativa < maxTentativas)
                        {
                            transacao.Rollback();

                            context.Entry(sale).State = EntityState.Detached;
                            foreach (var linha in sale.Lines)
                                context.Entry(linha).State = EntityState.Detached;
                        }
                    }
                }
            }

            throw new InvalidOperationException("Não foi possível gerar um número de venda único após várias tentativas. Tenta novamente.");
        }

        public void Atualizar(Sale sale)
        {
            using (var context = new CrmDbContext())
            {
                var existente = context.Sales
                    .Include(s => s.Lines)
                    .SingleOrDefault(s => s.SaleId == sale.SaleId && !s.IsDeleted);

                if (existente == null) return;

                if (sale.RowVersion != null && existente.RowVersion != null
                    && !existente.RowVersion.SequenceEqual(sale.RowVersion))
                {
                    throw new DbUpdateConcurrencyException(
                        "Esta venda foi alterada por outro utilizador entretanto. Recarrega a página antes de gravar novamente.");
                }

                existente.ClientId = sale.ClientId;
                existente.ProposalId = sale.ProposalId;
                existente.SaleDate = sale.SaleDate;
                existente.OwnerId = sale.OwnerId;
                existente.Origin = sale.Origin;
                existente.PaymentMethod = sale.PaymentMethod;
                existente.DueDate = sale.DueDate;
                existente.CommissionValue = sale.CommissionValue;
                existente.SubTotal = sale.SubTotal;
                existente.TaxTotal = sale.TaxTotal;
                existente.Total = sale.Total;
                existente.UpdatedDate = sale.UpdatedDate;
                existente.UpdatedBy = sale.UpdatedBy;

                var linhasAtivasExistentes = existente.Lines.Where(l => !l.IsDeleted).ToList();

                foreach (var linhaExistente in linhasAtivasExistentes)
                {
                    if (sale.Lines.All(l => l.SaleLineId != linhaExistente.SaleLineId))
                    {
                        linhaExistente.IsDeleted = true;
                        linhaExistente.DeletedDate = DateTime.UtcNow;
                        linhaExistente.DeletedBy = sale.UpdatedBy;
                    }
                }

                foreach (var linha in sale.Lines)
                {
                    var linhaExistente = linhasAtivasExistentes.SingleOrDefault(l => l.SaleLineId == linha.SaleLineId);
                    if (linhaExistente != null)
                    {
                        linhaExistente.ProductId = linha.ProductId;
                        linhaExistente.LineOrder = linha.LineOrder;
                        linhaExistente.Description = linha.Description;
                        linhaExistente.Quantity = linha.Quantity;
                        linhaExistente.UnitPrice = linha.UnitPrice;
                        linhaExistente.DiscountPercent = linha.DiscountPercent;
                        linhaExistente.TaxRateId = linha.TaxRateId;
                        linhaExistente.LineTotal = linha.LineTotal;
                        linhaExistente.UpdatedDate = DateTime.UtcNow;
                        linhaExistente.UpdatedBy = sale.UpdatedBy;
                    }
                    else
                    {
                        linha.SaleId = existente.SaleId;
                        linha.CreatedDate = DateTime.UtcNow;
                        linha.CreatedBy = sale.UpdatedBy;
                        context.SaleLines.Add(linha);
                    }
                }

                context.SaveChanges();
            }
        }

        public void AtualizarEstado(int saleId, string novoStatus, string cancellationReason, int userId)
        {
            using (var context = new CrmDbContext())
            {
                var sale = context.Sales.Find(saleId);
                if (sale == null) return;

                sale.Status = novoStatus;
                sale.CancellationReason = cancellationReason;
                sale.UpdatedDate = DateTime.UtcNow;
                sale.UpdatedBy = userId;

                context.SaveChanges();
            }
        }

        public void EliminarLogico(int saleId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var sale = context.Sales.Find(saleId);
                if (sale == null) return;

                sale.IsDeleted = true;
                sale.DeletedDate = DateTime.UtcNow;
                sale.DeletedBy = eliminadoPor;

                context.SaveChanges();
            }
        }

        // Formato ASSUMIDO, espelha o de Proposals: VEN-{ano}-{sequencial 4 dígitos}. Reinicia a cada ano.
        private string GerarProximoNumero(CrmDbContext context)
        {
            int ano = DateTime.Today.Year;
            string prefixo = $"VEN-{ano}-";

            int ultimoNumero = context.Sales
                .Where(s => s.SaleNumber.StartsWith(prefixo))
                .Select(s => s.SaleNumber)
                .ToList()
                .Select(n => int.Parse(n.Substring(prefixo.Length)))
                .DefaultIfEmpty(0)
                .Max();

            return $"{prefixo}{(ultimoNumero + 1):D4}";
        }
    }
}