using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Notificacoes;

namespace CRM.Services
{
    public class EmailTemplateService
    {
        private readonly EmailTemplateRepository _emailTemplateRepository = new EmailTemplateRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(EmailTemplate template, bool nomeJaExiste)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(template.Name))
                erros.Add("O nome do modelo é obrigatório.");
            else if (template.Name.Trim().Length > 150)
                erros.Add("O nome não pode exceder 150 caracteres.");

            if (nomeJaExiste)
                erros.Add("Já existe um modelo com este nome.");

            if (string.IsNullOrWhiteSpace(template.Subject))
                erros.Add("O assunto é obrigatório.");

            if (string.IsNullOrWhiteSpace(template.Body))
                erros.Add("O corpo do email é obrigatório.");

            return erros;
        }

        public EmailTemplate GetById(int emailTemplateId) => _emailTemplateRepository.GetById(emailTemplateId);

        public List<EmailTemplate> Listar(string pesquisa) => _emailTemplateRepository.Listar(pesquisa);

        public List<EmailTemplate> ListarAtivos() => _emailTemplateRepository.ListarAtivos();

        public bool ExisteNome(string name, int? ignorarEmailTemplateId = null) =>
            _emailTemplateRepository.ExisteNome(name, ignorarEmailTemplateId);

        public int Criar(EmailTemplate template)
        {
            int id = _emailTemplateRepository.Criar(template);
            _auditService.Registar(template.CreatedBy, "Create", "EmailTemplate", id.ToString(),
                $"Modelo de email '{template.Name}' criado.");
            return id;
        }

        public void Atualizar(EmailTemplate template)
        {
            _emailTemplateRepository.Atualizar(template);
            _auditService.Registar(template.UpdatedBy, "Update", "EmailTemplate", template.EmailTemplateId.ToString(),
                $"Modelo de email '{template.Name}' atualizado.");
        }

        public void AlternarEstado(int emailTemplateId, int alteradoPor)
        {
            _emailTemplateRepository.AlternarEstado(emailTemplateId, alteradoPor);
            _auditService.Registar(alteradoPor, "AlternarEstado", "EmailTemplate", emailTemplateId.ToString(),
                "Estado do modelo de email alternado (ativo/inativo).");
        }
        public string SubstituirVariaveis(string texto, Dictionary<string, string> variaveis)
        {
            if (string.IsNullOrEmpty(texto) || variaveis == null) return texto;

            foreach (var par in variaveis)
                texto = texto.Replace("{{" + par.Key + "}}", par.Value ?? "");

            return texto;
        }
    }
}