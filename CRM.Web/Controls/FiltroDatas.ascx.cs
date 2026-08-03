using System;

namespace CRM.Web.Controls
{
    public partial class FiltroDatas : System.Web.UI.UserControl
    {
        public DateTime? DataInicial
        {
            get => DateTime.TryParse(txtDataInicial.Text, out var d) ? d : (DateTime?)null;
            set => txtDataInicial.Text = value?.ToString("yyyy-MM-dd");
        }

        public DateTime? DataFinal
        {
            get => DateTime.TryParse(txtDataFinal.Text, out var d) ? d : (DateTime?)null;
            set => txtDataFinal.Text = value?.ToString("yyyy-MM-dd");
        }

        public bool Valido => !DataInicial.HasValue || !DataFinal.HasValue || DataFinal >= DataInicial;
    }
}