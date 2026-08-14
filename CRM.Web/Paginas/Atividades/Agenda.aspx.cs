using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;
using CRM.Services;

namespace CRM.Web.Paginas.Atividades
{
    public partial class Agenda : PaginaBase
    {
        private readonly ActivityService _activityService = new ActivityService();
        private readonly UserRepository _userRepository = new UserRepository();
        private static readonly CultureInfo PtPt = new CultureInfo("pt-PT");

        protected bool PodeCriarAtividade => _activityService.PodeCriar(Perfil);

        private DateTime DataReferencia
        {
            get => ViewState["DataReferencia"] as DateTime? ?? DateTime.Today;
            set => ViewState["DataReferencia"] = value.Date;
        }

        private string Vista
        {
            get => ViewState["Vista"] as string ?? "Mes";
            set => ViewState["Vista"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkNova.Visible = PodeCriarAtividade;

            if (!IsPostBack)
            {
                CarregarResponsaveis();
                CarregarAgenda();
            }
        }

        private void CarregarResponsaveis()
        {
            bool podeFiltrarPorResponsavel = !_activityService.TemAmbitoProprios(Perfil);

            ddlResponsavel.Visible = podeFiltrarPorResponsavel;
            lblResponsavel.Visible = podeFiltrarPorResponsavel;

            if (!podeFiltrarPorResponsavel) return;

            ddlResponsavel.Items.Add(new ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarAtivos())
                ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
        }

        private int? ObterFiltroResponsavel()
        {
            if (_activityService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlResponsavel.Visible && !string.IsNullOrEmpty(ddlResponsavel.SelectedValue))
                return int.Parse(ddlResponsavel.SelectedValue);

            return null;
        }

        private ActivityFiltro ObterFiltro() => new ActivityFiltro
        {
            Tipo = ddlTipo.SelectedValue,
            Status = ddlEstado.SelectedValue,
            AssignedToUserId = ObterFiltroResponsavel()
        };

        protected void btnFiltrar_Click(object sender, EventArgs e) => CarregarAgenda();

        protected void lnkVista_Command(object sender, CommandEventArgs e)
        {
            Vista = e.CommandArgument.ToString();
            CarregarAgenda();
        }

        protected void lnkAnterior_Click(object sender, EventArgs e)
        {
            DataReferencia = Deslocar(DataReferencia, -1);
            CarregarAgenda();
        }

        protected void lnkSeguinte_Click(object sender, EventArgs e)
        {
            DataReferencia = Deslocar(DataReferencia, 1);
            CarregarAgenda();
        }

        protected void lnkHoje_Click(object sender, EventArgs e)
        {
            DataReferencia = DateTime.Today;
            CarregarAgenda();
        }

        private DateTime Deslocar(DateTime data, int sentido)
        {
            switch (Vista)
            {
                case "Dia": return data.AddDays(1 * sentido);
                case "Semana": return data.AddDays(7 * sentido);
                default: return data.AddMonths(1 * sentido);
            }
        }

        private DateTime InicioDaSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Monday)) % 7;
            return data.Date.AddDays(-diff);
        }

        private void CarregarAgenda()
        {
            AtualizarBotoesVista();

            phMes.Visible = Vista == "Mes";
            phSemana.Visible = Vista == "Semana";
            phDia.Visible = Vista == "Dia";

            switch (Vista)
            {
                case "Dia": CarregarVistaDia(); break;
                case "Semana": CarregarVistaSemana(); break;
                default: CarregarVistaMes(); break;
            }
        }

        private void AtualizarBotoesVista()
        {
            lnkVistaDia.CssClass = "btn btn-sm " + (Vista == "Dia" ? "btn-secondary" : "btn-outline-secondary");
            lnkVistaSemana.CssClass = "btn btn-sm " + (Vista == "Semana" ? "btn-secondary" : "btn-outline-secondary");
            lnkVistaMes.CssClass = "btn btn-sm " + (Vista == "Mes" ? "btn-secondary" : "btn-outline-secondary");
        }

        private void CarregarVistaMes()
        {
            var primeiroDiaMes = new DateTime(DataReferencia.Year, DataReferencia.Month, 1);
            var inicioGrelha = InicioDaSemana(primeiroDiaMes);
            var fimGrelha = inicioGrelha.AddDays(42);

            var atividades = _activityService.ListarPorPeriodo(inicioGrelha, fimGrelha, ObterFiltro(), UserId, Perfil);
            var porDia = AgruparPorDia(atividades);

            var semanas = new List<SemanaCalendarioViewModel>();
            for (int semana = 0; semana < 6; semana++)
            {
                var dias = new List<DiaCalendarioViewModel>();
                for (int dia = 0; dia < 7; dia++)
                {
                    var data = inicioGrelha.AddDays(semana * 7 + dia);
                    dias.Add(new DiaCalendarioViewModel
                    {
                        Data = data,
                        ForaDoMes = data.Month != DataReferencia.Month,
                        EhHoje = data == DateTime.Today,
                        Atividades = porDia.TryGetValue(data, out var lista) ? lista : new List<Activity>()
                    });
                }
                semanas.Add(new SemanaCalendarioViewModel { Dias = dias });
            }

            rptSemanasMes.DataSource = semanas;
            rptSemanasMes.DataBind();

            litPeriodo.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DataReferencia.ToString("MMMM yyyy", PtPt));
        }

        private void CarregarVistaSemana()
        {
            var inicioSemana = InicioDaSemana(DataReferencia);
            var fimSemana = inicioSemana.AddDays(7);

            var atividades = _activityService.ListarPorPeriodo(inicioSemana, fimSemana, ObterFiltro(), UserId, Perfil);
            var porDia = AgruparPorDia(atividades);

            var dias = new List<DiaCalendarioViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var data = inicioSemana.AddDays(i);
                dias.Add(new DiaCalendarioViewModel
                {
                    Data = data,
                    EhHoje = data == DateTime.Today,
                    Atividades = porDia.TryGetValue(data, out var lista) ? lista : new List<Activity>()
                });
            }

            rptCabecalhoSemana.DataSource = dias;
            rptCabecalhoSemana.DataBind();

            rptDiasSemana.DataSource = dias;
            rptDiasSemana.DataBind();

            litPeriodo.Text = $"{inicioSemana:dd MMM} – {inicioSemana.AddDays(6):dd MMM yyyy}";
        }

        private void CarregarVistaDia()
        {
            var inicioDia = DataReferencia.Date;
            var fimDia = inicioDia.AddDays(1);

            var atividades = _activityService
                .ListarPorPeriodo(inicioDia, fimDia, ObterFiltro(), UserId, Perfil)
                .OrderBy(a => a.StartDateTime)
                .ToList();

            rptAtividadesDia.DataSource = atividades;
            rptAtividadesDia.DataBind();

            phDiaVazio.Visible = atividades.Count == 0;

            litPeriodo.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                DataReferencia.ToString("dddd, dd 'de' MMMM", PtPt));
        }

        private Dictionary<DateTime, List<Activity>> AgruparPorDia(List<Activity> atividades)
        {
            return atividades
                .GroupBy(a => a.StartDateTime.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(a => a.StartDateTime).ToList());
        }

        // ===================== Helpers de markup (evitam ternários/concatenação inline no .aspx) =====================

        protected string GetDiaCellClass(object dataItem)
        {
            var dia = (DiaCalendarioViewModel)dataItem;
            string classe = "crm-calendar-dia";
            if (dia.ForaDoMes) classe += " text-muted bg-light";
            if (dia.EhHoje) classe += " crm-calendar-hoje";
            return classe;
        }

        protected string GetHojeHeaderClass(object dataItem)
        {
            var dia = (DiaCalendarioViewModel)dataItem;
            return dia.EhHoje ? "crm-calendar-hoje" : "";
        }

        protected string GetAddLinkStyle()
        {
            return PodeCriarAtividade ? "display:inline;" : "display:none;";
        }

        protected string GetRelacionado(object dataItem)
        {
            var activity = (Activity)dataItem;

            if (activity.RelatedClientId.HasValue)
                return "Cliente: " + activity.RelatedClient?.TradeName;

            if (activity.RelatedLeadId.HasValue)
                return "Lead: " + activity.RelatedLead?.Name;

            if (activity.RelatedOpportunityId.HasValue)
                return "Oportunidade #" + activity.RelatedOpportunityId;

            return "";
        }

        protected string GetHorario(object dataItem)
        {
            var activity = (Activity)dataItem;
            return activity.EndDateTime.HasValue
                ? $"{activity.StartDateTime:HH:mm} - {activity.EndDateTime:HH:mm}"
                : activity.StartDateTime.ToString("HH:mm");
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Planeada": return "bg-secondary";
                case "Em Curso": return "badge-em-contacto";
                case "Concluída": return "badge-ativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }
    }

    public class SemanaCalendarioViewModel
    {
        public List<DiaCalendarioViewModel> Dias { get; set; }
    }

    public class DiaCalendarioViewModel
    {
        public DateTime Data { get; set; }
        public bool ForaDoMes { get; set; }
        public bool EhHoje { get; set; }
        public List<Activity> Atividades { get; set; }
    }
}