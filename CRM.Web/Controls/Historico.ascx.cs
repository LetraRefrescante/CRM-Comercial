using CRM.Services;
using CRM.Data.Repositories;
using System.Linq;

namespace CRM.Web.Controls
{
    public partial class Historico : System.Web.UI.UserControl
    {
        private readonly AuditService _auditService = new AuditService();
        private readonly UserRepository _userRepository = new UserRepository();
        public void Inicializar(string entityName, string entityId)
        {
            var registos = _auditService.Listar(entityName, entityId);
            var utilizadores = _userRepository.ListarComerciaisAtivos()
                .ToDictionary(u => u.UserId, u => u.Name);

            var modeloVista = registos.Select(r => new
            {
                r.Action,
                r.CreatedDate,
                NomeUtilizador = r.UserId.HasValue && utilizadores.ContainsKey(r.UserId.Value)
                    ? utilizadores[r.UserId.Value]
                    : "Sistema"
            }).ToList();

            rptHistorico.DataSource = modeloVista;
            rptHistorico.DataBind();
            phVazio.Visible = modeloVista.Count == 0;
        }
    }
}