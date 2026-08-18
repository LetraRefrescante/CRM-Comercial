using CRM.Data.Context;
using CRM.Models.Entities.Oportunidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class OpportunityRepository
    {
        public Opportunity GetById(int opportunityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Opportunities
                    .Include(o => o.Client)
                    .Include(o => o.Contact)
                    .Include(o => o.Stage)
                    .Include(o => o.Owner)
                    .Include(o => o.LossReason)
                    .FirstOrDefault(o => o.OpportunityId == opportunityId && !o.IsDeleted);
            }
        }
        public List<Opportunity> ListarPorCliente(int clientId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Opportunities
                    .Where(o => o.ClientId == clientId && !o.IsDeleted)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToList();
            }
        }

        public List<Opportunity> ListarParaSelecao()
        {
            using (var context = new CrmDbContext())
            {
                return context.Opportunities
                    .Where(o => !o.IsDeleted)
                    .OrderBy(o => o.Title)
                    .ToList();
            }
        }

        public List<Opportunity> Listar(
            string pesquisa,
            int? stageId,
            int? ownerId,
            int? clientId,
            bool? isClosed,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Opportunities
                    .Include(o => o.Client)
                    .Include(o => o.Stage)
                    .Include(o => o.Owner)
                    .Where(o => !o.IsDeleted);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(o => o.Title.Contains(pesquisa) || o.Client.TradeName.Contains(pesquisa));

                if (stageId.HasValue)
                    query = query.Where(o => o.StageId == stageId.Value);

                if (ownerId.HasValue)
                    query = query.Where(o => o.OwnerId == ownerId.Value);

                if (clientId.HasValue)
                    query = query.Where(o => o.ClientId == clientId.Value);

                if (isClosed.HasValue)
                    query = query.Where(o => o.IsClosed == isClosed.Value);

                query = query.OrderByDescending(o => o.CreatedDate);

                totalRegistos = query.Count();

                return query
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }
        public List<Opportunity> ListarAbertasParaPipeline(int? ownerId)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Opportunities
                    .Include(o => o.Client)
                    .Include(o => o.Stage)
                    .Include(o => o.Owner)
                    .Where(o => !o.IsDeleted && !o.IsClosed);

                if (ownerId.HasValue)
                    query = query.Where(o => o.OwnerId == ownerId.Value);

                return query.OrderByDescending(o => o.EstimatedValue).ToList();
            }
        }
        public List<Opportunity> ListarSemAtividadeRecente(int diasAlerta, int? ownerId)
        {
            using (var context = new CrmDbContext())
            {
                var limite = DateTime.UtcNow.AddDays(-diasAlerta);

                var query = context.Opportunities
                    .Include(o => o.Client)
                    .Include(o => o.Stage)
                    .Where(o => !o.IsDeleted && !o.IsClosed);

                if (ownerId.HasValue)
                    query = query.Where(o => o.OwnerId == ownerId.Value);

                return query
                    .Where(o => !context.Activities.Any(a =>
                        a.RelatedOpportunityId == o.OpportunityId &&
                        !a.IsDeleted &&
                        a.StartDateTime >= limite))
                    .OrderByDescending(o => o.EstimatedValue)
                    .ToList();
            }
        }
        public void Adicionar(Opportunity opportunity)
        {
            using (var context = new CrmDbContext())
            {
                context.Opportunities.Add(opportunity);
                context.SaveChanges();
            }
        }
        public void Atualizar(Opportunity opportunityAtualizada)
        {
            using (var context = new CrmDbContext())
            {
                var opportunity = context.Opportunities.Find(opportunityAtualizada.OpportunityId);
                if (opportunity == null) return;

                opportunity.Title = opportunityAtualizada.Title;
                opportunity.ClientId = opportunityAtualizada.ClientId;
                opportunity.ContactId = opportunityAtualizada.ContactId;
                opportunity.StageId = opportunityAtualizada.StageId;
                opportunity.EstimatedValue = opportunityAtualizada.EstimatedValue;
                opportunity.Probability = opportunityAtualizada.Probability;
                opportunity.ExpectedCloseDate = opportunityAtualizada.ExpectedCloseDate;
                opportunity.OwnerId = opportunityAtualizada.OwnerId;
                opportunity.Competitor = opportunityAtualizada.Competitor;
                opportunity.IsClosed = opportunityAtualizada.IsClosed;
                opportunity.ClosedDate = opportunityAtualizada.ClosedDate;
                opportunity.LossReasonId = opportunityAtualizada.LossReasonId;
                opportunity.UpdatedDate = opportunityAtualizada.UpdatedDate;
                opportunity.UpdatedBy = opportunityAtualizada.UpdatedBy;

                context.Entry(opportunity).OriginalValues["RowVersion"] = opportunityAtualizada.RowVersion;

                context.SaveChanges();
            }
        }

        public void RegistarHistoricoFase(OpportunityStageHistory registo)
        {
            using (var context = new CrmDbContext())
            {
                context.OpportunityStageHistories.Add(registo);
                context.SaveChanges();
            }
        }

        public List<OpportunityStageHistory> ListarHistoricoFases(int opportunityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.OpportunityStageHistories
                    .Include(h => h.PreviousStage)
                    .Include(h => h.NewStage)
                    .Where(h => h.OpportunityId == opportunityId)
                    .OrderByDescending(h => h.ChangedDate)
                    .ToList();
            }
        }
    }
}