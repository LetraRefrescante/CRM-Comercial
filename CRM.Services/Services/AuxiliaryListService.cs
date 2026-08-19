using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.ListasAuxiliares;
using System.Collections.Generic;

namespace CRM.Services
{
    public class AuxiliaryListService
    {
        private readonly SectorRepository _sectorRepository = new SectorRepository();
        private readonly LeadSourceRepository _leadSourceRepository = new LeadSourceRepository();
        private readonly LossReasonRepository _lossReasonRepository = new LossReasonRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly PaymentTermRepository _paymentTermRepository = new PaymentTermRepository();
        private readonly CountryRepository _countryRepository = new CountryRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Configuracoes";

        public bool PodeGerir(string perfil) => _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Total;
        public bool PodeConsultar(string perfil) => _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Consulta;

        // ===================== Setores =====================
        public List<Sector> ListarSetores(bool incluirInativos) => _sectorRepository.Listar(incluirInativos);

        public string CriarSetor(string nome, int userId)
        {
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (_sectorRepository.ExisteNome(nome)) return "Já existe um setor com este nome.";

            int id = _sectorRepository.Criar(new Sector { Name = nome, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "Sector", id.ToString(), $"Setor '{nome}' criado.");
            return null;
        }

        public void AlternarEstadoSetor(int id, int userId)
        {
            _sectorRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "Sector", id.ToString());
        }

        // ===================== Origens de Lead =====================
        public List<LeadSource> ListarOrigensLead(bool incluirInativos) => _leadSourceRepository.Listar(incluirInativos);

        public string CriarOrigemLead(string nome, int userId)
        {
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (_leadSourceRepository.ExisteNome(nome)) return "Já existe uma origem com este nome.";

            int id = _leadSourceRepository.Criar(new LeadSource { Name = nome, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "LeadSource", id.ToString(), $"Origem de lead '{nome}' criada.");
            return null;
        }

        public void AlternarEstadoOrigemLead(int id, int userId)
        {
            _leadSourceRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "LeadSource", id.ToString());
        }

        // ===================== Motivos de Perda =====================
        public List<LossReason> ListarMotivosPerda(bool incluirInativos) =>
            _lossReasonRepository.Listar(null, incluirInativos);

        public string CriarMotivoPerda(string nome, int userId)
        {
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (_lossReasonRepository.ExisteNome(nome)) return "Já existe um motivo com este nome.";

            int id = _lossReasonRepository.Criar(new LossReason { Name = nome, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "LossReason", id.ToString(), $"Motivo de perda '{nome}' criado.");
            return null;
        }

        public void AlternarEstadoMotivoPerda(int id, int userId)
        {
            _lossReasonRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "LossReason", id.ToString());
        }

        // ===================== Taxas de IVA =====================
        public List<TaxRate> ListarTaxasIva(bool incluirInativos) =>
            incluirInativos ? _taxRateRepository.ListarTodas() : _taxRateRepository.ListarAtivas();

        public string CriarTaxaIva(string nome, decimal percentagem, int userId)
        {
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (percentagem < 0 || percentagem > 100) return "A percentagem tem de estar entre 0 e 100.";
            if (_taxRateRepository.ExisteNome(nome)) return "Já existe uma taxa com este nome.";

            int id = _taxRateRepository.Criar(new TaxRate { Name = nome, Percentage = percentagem, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "TaxRate", id.ToString(), $"Taxa de IVA '{nome}' ({percentagem}%) criada.");
            return null;
        }

        public void AlternarEstadoTaxaIva(int id, int userId)
        {
            _taxRateRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "TaxRate", id.ToString());
        }

        // ===================== Condições de Pagamento =====================
        public List<PaymentTerm> ListarCondicoesPagamento(bool incluirInativos) =>
            _paymentTermRepository.Listar(null, incluirInativos);

        public string CriarCondicaoPagamento(string nome, int? diasVencimento, int userId)
        {
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (diasVencimento.HasValue && diasVencimento < 0) return "Os dias até vencimento não podem ser negativos.";
            if (_paymentTermRepository.ExisteNome(nome)) return "Já existe uma condição com este nome.";

            int id = _paymentTermRepository.Criar(new PaymentTerm { Name = nome, DaysDue = diasVencimento, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "PaymentTerm", id.ToString(), $"Condição de pagamento '{nome}' criada.");
            return null;
        }

        public void AlternarEstadoCondicaoPagamento(int id, int userId)
        {
            _paymentTermRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "PaymentTerm", id.ToString());
        }

        // ===================== Países =====================
        public List<Country> ListarPaises(bool incluirInativos) => _countryRepository.Listar(incluirInativos);

        public string CriarPais(string codigo, string nome, int userId)
        {
            codigo = codigo?.Trim().ToUpperInvariant();
            nome = nome?.Trim();
            if (string.IsNullOrEmpty(codigo) || codigo.Length > 3) return "O código tem de ter entre 1 e 3 letras.";
            if (string.IsNullOrEmpty(nome)) return "O nome é obrigatório.";
            if (_countryRepository.ExisteCodigo(codigo)) return "Já existe um país com este código.";

            int id = _countryRepository.Criar(new Country { IsoCode = codigo, Name = nome, CreatedBy = userId });
            _auditService.Registar(userId, "Create", "Country", id.ToString(), $"País '{nome}' ({codigo}) criado.");
            return null;
        }

        public void AlternarEstadoPais(int id, int userId)
        {
            _countryRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "Country", id.ToString());
        }
    }
}