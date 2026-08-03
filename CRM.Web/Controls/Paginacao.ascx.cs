using System;

namespace CRM.Web.Controls
{
    public partial class Paginacao : System.Web.UI.UserControl
    {
        public event EventHandler<EventArgs> PaginaAlterada;

        public int TotalRegistos { get; set; }
        public int TamanhoPagina { get; set; } = 20;

        public int PaginaAtual
        {
            get => ViewState["PaginaAtual"] == null ? 1 : (int)ViewState["PaginaAtual"];
            set => ViewState["PaginaAtual"] = value;
        }

        public int TotalPaginas => TotalRegistos == 0
            ? 1
            : (int)Math.Ceiling(TotalRegistos / (double)TamanhoPagina);

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AtualizarVisual();
        }

        private void AtualizarVisual()
        {
            litResumo.Text = TotalRegistos == 0
                ? "Sem registos"
                : $"A mostrar {IndiceInicial}–{IndiceFinal} de {TotalRegistos}";

            litPaginaAtual.Text = $"{PaginaAtual} / {TotalPaginas}";

            lnkPrimeira.Enabled = PaginaAtual > 1;
            lnkAnterior.Enabled = PaginaAtual > 1;
            lnkSeguinte.Enabled = PaginaAtual < TotalPaginas;
            lnkUltima.Enabled = PaginaAtual < TotalPaginas;
        }

        private int IndiceInicial => (PaginaAtual - 1) * TamanhoPagina + 1;
        private int IndiceFinal => Math.Min(PaginaAtual * TamanhoPagina, TotalRegistos);

        protected void lnkPrimeira_Click(object sender, EventArgs e) => IrPara(1);
        protected void lnkAnterior_Click(object sender, EventArgs e) => IrPara(PaginaAtual - 1);
        protected void lnkSeguinte_Click(object sender, EventArgs e) => IrPara(PaginaAtual + 1);
        protected void lnkUltima_Click(object sender, EventArgs e) => IrPara(TotalPaginas);

        private void IrPara(int pagina)
        {
            PaginaAtual = Math.Max(1, Math.Min(pagina, TotalPaginas));
            PaginaAlterada?.Invoke(this, EventArgs.Empty);
        }
    }
}