using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class ProposalRepository
    {
        public Proposal GetById(int proposalId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Proposals
                    .Include(p => p.Client)
                    .Include(p => p.Client.AccountManager)
                    .Include(p => p.Opportunity)
                    .Include(p => p.PaymentTerm)
                    .Include(p => p.AcceptedByUser)
                    .Include(p => p.Lines.Select(l => l.Product))
                    .Include(p => p.Lines.Select(l => l.TaxRate))
                    .Where(p => p.ProposalId == proposalId && !p.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public List<Proposal> Listar(
            string pesquisa,
            string status,
            int? clientId,
            int? accountManagerId,
            DateTime? dataInicio,
            DateTime? dataFim,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos,
            string sortColumn = "IssueDate",
            bool sortAscending = false)
        {
            using (var context = new CrmDbContext())
            {
                var query = ConstruirQuery(context, pesquisa, status, clientId, accountManagerId, dataInicio, dataFim);

                totalRegistos = query.Count();

                return AplicarOrdenacao(query, sortColumn, sortAscending)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        private IQueryable<Proposal> ConstruirQuery(
            CrmDbContext context,
            string pesquisa,
            string status,
            int? clientId,
            int? accountManagerId,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var query = context.Proposals
                .Include(p => p.Client)
                .Include(p => p.Client.AccountManager)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(p =>
                    p.ProposalNumber.Contains(pesquisa) ||
                    p.Client.TradeName.Contains(pesquisa));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            if (clientId.HasValue)
            {
                query = query.Where(p => p.ClientId == clientId.Value);
            }

            if (accountManagerId.HasValue)
            {
                query = query.Where(p => p.Client.AccountManagerId == accountManagerId.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(p => p.IssueDate >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(p => p.IssueDate <= dataFim.Value.Date);
            }

            return query;
        }

        private IQueryable<Proposal> AplicarOrdenacao(IQueryable<Proposal> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "ProposalNumber":
                    return sortAscending ? query.OrderBy(p => p.ProposalNumber) : query.OrderByDescending(p => p.ProposalNumber);
                case "Client":
                    return sortAscending ? query.OrderBy(p => p.Client.TradeName) : query.OrderByDescending(p => p.Client.TradeName);
                case "ValidUntil":
                    return sortAscending ? query.OrderBy(p => p.ValidUntil) : query.OrderByDescending(p => p.ValidUntil);
                case "Status":
                    return sortAscending ? query.OrderBy(p => p.Status) : query.OrderByDescending(p => p.Status);
                case "Total":
                    return sortAscending ? query.OrderBy(p => p.Total) : query.OrderByDescending(p => p.Total);
                case "IssueDate":
                default:
                    return sortAscending ? query.OrderBy(p => p.IssueDate) : query.OrderByDescending(p => p.IssueDate);
            }
        }

        public void EliminarLogico(int proposalId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var proposal = context.Proposals.Find(proposalId);
                if (proposal == null) return;

                proposal.IsDeleted = true;
                proposal.DeletedDate = DateTime.UtcNow;
                proposal.DeletedBy = eliminadoPor;

                context.SaveChanges();
            }
        }

        public Proposal Criar(Proposal proposal)
        {
            using (var context = new CrmDbContext())
            {
                proposal.ProposalNumber = GerarProximoNumero(context);
                context.Proposals.Add(proposal);
                context.SaveChanges();
                return proposal;
            }
        }

        // Atribuição explícita campo-a-campo. NÃO usar CurrentValues.SetValues(proposal) aqui:
        // isso copiaria também CreatedDate/CreatedBy/RowVersion do objeto recebido (que vêm a
        // default, não preenchidos no code-behind), apagando a auditoria de criação e mexendo
        // no token de concorrência otimista.
        public void Atualizar(Proposal proposal)
        {
            using (var context = new CrmDbContext())
            {
                var existente = context.Proposals
                    .Include(p => p.Lines)
                    .SingleOrDefault(p => p.ProposalId == proposal.ProposalId && !p.IsDeleted);

                if (existente == null) return;

                existente.ClientId = proposal.ClientId;
                existente.OpportunityId = proposal.OpportunityId;
                existente.IssueDate = proposal.IssueDate;
                existente.ValidUntil = proposal.ValidUntil;
                existente.Status = proposal.Status;
                existente.GlobalDiscountPercent = proposal.GlobalDiscountPercent;
                existente.PaymentTermId = proposal.PaymentTermId;
                existente.Notes = proposal.Notes;
                existente.SubTotal = proposal.SubTotal;
                existente.TaxTotal = proposal.TaxTotal;
                existente.Total = proposal.Total;
                existente.UpdatedDate = proposal.UpdatedDate;
                existente.UpdatedBy = proposal.UpdatedBy;

                foreach (var linhaExistente in existente.Lines.ToList())
                {
                    if (proposal.Lines.All(l => l.ProposalLineId != linhaExistente.ProposalLineId))
                        context.ProposalLines.Remove(linhaExistente);
                }

                foreach (var linha in proposal.Lines)
                {
                    var linhaExistente = existente.Lines.SingleOrDefault(l => l.ProposalLineId == linha.ProposalLineId);
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
                    }
                    else
                    {
                        linha.ProposalId = existente.ProposalId;
                        context.ProposalLines.Add(linha);
                    }
                }

                context.SaveChanges();
            }
        }

        public Proposal CriarNovaVersao(int proposalIdOrigem, int userId)
        {
            using (var context = new CrmDbContext())
            {
                var original = context.Proposals
                    .Include(p => p.Lines)
                    .SingleOrDefault(p => p.ProposalId == proposalIdOrigem && !p.IsDeleted);

                if (original == null) return null;

                int proposalIdRaiz = original.ParentProposalId ?? original.ProposalId;

                int versaoAtual = context.Proposals
                    .Where(p => p.ProposalId == proposalIdRaiz || p.ParentProposalId == proposalIdRaiz)
                    .Max(p => (int?)p.VersionNumber) ?? original.VersionNumber;

                int duracaoValidadeDias = (original.ValidUntil - original.IssueDate).Days;

                var novaVersao = new Proposal
                {
                    ProposalNumber = GerarProximoNumero(context),
                    ClientId = original.ClientId,
                    OpportunityId = original.OpportunityId,
                    IssueDate = DateTime.Today,
                    ValidUntil = DateTime.Today.AddDays(duracaoValidadeDias),
                    Status = "Rascunho",
                    GlobalDiscountPercent = original.GlobalDiscountPercent,
                    PaymentTermId = original.PaymentTermId,
                    Notes = original.Notes,
                    ParentProposalId = proposalIdRaiz,
                    VersionNumber = versaoAtual + 1,
                    CreatedBy = userId,
                    Lines = original.Lines.Select(l => new ProposalLine
                    {
                        ProductId = l.ProductId,
                        LineOrder = l.LineOrder,
                        Description = l.Description,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        DiscountPercent = l.DiscountPercent,
                        TaxRateId = l.TaxRateId,
                        LineTotal = l.LineTotal
                    }).ToList()
                };

                context.Proposals.Add(novaVersao);
                context.SaveChanges();
                return novaVersao;
            }
        }

        public string GerarProximoNumero()
        {
            using (var context = new CrmDbContext())
            {
                return GerarProximoNumero(context);
            }
        }

        // Formato ASSUMIDO: PROP-{ano}-{sequencial 4 dígitos}, reinicia a cada ano. Confirma.
        private string GerarProximoNumero(CrmDbContext context)
        {
            int ano = DateTime.Today.Year;
            string prefixo = $"PROP-{ano}-";

            int ultimoNumero = context.Proposals
                .Where(p => p.ProposalNumber.StartsWith(prefixo))
                .Select(p => p.ProposalNumber)
                .ToList()
                .Select(n => int.Parse(n.Substring(prefixo.Length)))
                .DefaultIfEmpty(0)
                .Max();

            return $"{prefixo}{(ultimoNumero + 1):D4}";
        }
    }
}