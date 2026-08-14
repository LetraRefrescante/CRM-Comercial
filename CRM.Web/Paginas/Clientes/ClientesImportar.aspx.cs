using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CRM.Services;
using CRM.Data.Context;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClientesImportar : PaginaBase
    {
        private const long TamanhoMaximoBytes = 2 * 1024 * 1024; // 2 MB

        private readonly ClientService _clientService = new ClientService();
        private readonly UserRepository _userRepository = new UserRepository();

        private static readonly string[] EstadosValidos = { "Potencial", "Ativo", "Inativo", "Bloqueado" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_clientService.PodeCriarOuEditar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
            }
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!fileImportar.HasFile)
            {
                NotificacaoService.Erro("Selecione um ficheiro CSV.");
                return;
            }

            if (!fileImportar.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                NotificacaoService.Erro("Apenas ficheiros .csv são aceites.");
                return;
            }

            if (fileImportar.PostedFile.ContentLength > TamanhoMaximoBytes)
            {
                NotificacaoService.Erro("Ficheiro demasiado grande (máximo 2 MB).");
                return;
            }

            var erros = new List<string>();
            int sucesso = 0;
            int numeroLinha = 0;

            using (var reader = new StreamReader(fileImportar.FileContent, Encoding.UTF8))
            {
                bool primeiraLinha = true;
                string linha;

                while ((linha = reader.ReadLine()) != null)
                {
                    numeroLinha++;

                    if (primeiraLinha)
                    {
                        primeiraLinha = false;
                        continue; // salta o cabeçalho
                    }

                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    var campos = linha.Split(';');

                    if (campos.Length < 12)
                    {
                        erros.Add($"Linha {numeroLinha}: número de colunas inválido (esperadas 12 ou 13).");
                        continue;
                    }

                    try
                    {
                        var client = ConstruirClientDoCsv(campos);
                        var resultado = _clientService.Criar(client, Perfil, UserId);

                        switch (resultado)
                        {
                            case ResultadoGuardarCliente.Sucesso:
                                sucesso++;
                                break;
                            case ResultadoGuardarCliente.NifDuplicado:
                                erros.Add($"Linha {numeroLinha}: já existe um cliente ativo com o NIF '{client.VatNumber}'.");
                                break;
                            case ResultadoGuardarCliente.SemPermissao:
                                erros.Add($"Linha {numeroLinha}: sem permissão para importar.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Linha {numeroLinha}: {ex.Message}");
                    }
                }
            }

            litResumo.Text = $"{sucesso} cliente(s) importado(s) com sucesso. {erros.Count} linha(s) com erro.";
            phResumo.Visible = true;

            phErros.Visible = erros.Count > 0;
            rptErros.DataSource = erros;
            rptErros.DataBind();

            if (sucesso > 0)
            {
                NotificacaoService.Sucesso($"{sucesso} cliente(s) importado(s).");
            }
        }

        private Client ConstruirClientDoCsv(string[] campos)
        {
            string nomeComercial = campos[0].Trim();
            string nomeLegal = campos[1].Trim();
            string nif = campos[2].Trim();
            string email = campos[3].Trim();
            string telefone = campos[4].Trim();
            string morada = campos[5].Trim();
            string codigoPostal = campos[6].Trim();
            string cidade = campos[7].Trim();
            string paisIso = campos[8].Trim().ToUpperInvariant();
            string setorNome = campos[9].Trim();
            string emailComercial = campos[10].Trim();
            string estado = campos[11].Trim();
            string observacoes = campos.Length > 12 ? campos[12].Trim() : "";

            if (string.IsNullOrWhiteSpace(nomeComercial) || nomeComercial.Length < 2 || nomeComercial.Length > 150)
                throw new Exception("Nome Comercial deve ter entre 2 e 150 caracteres.");

            if (string.IsNullOrWhiteSpace(nif))
                throw new Exception("NIF é obrigatório.");

            int countryId;
            bool paisEhPortugal;
            int? sectorId = null;

            using (var context = new CrmDbContext())
            {
                var pais = context.Countries.SingleOrDefault(c => c.IsoCode == paisIso && c.IsActive);
                if (pais == null)
                    throw new Exception($"País com código '{paisIso}' não encontrado ou inativo.");
                countryId = pais.CountryId;
                paisEhPortugal = pais.IsoCode == "PT";

                if (!string.IsNullOrWhiteSpace(setorNome))
                {
                    var setor = context.Sectors.SingleOrDefault(s => s.Name == setorNome && s.IsActive);
                    sectorId = setor?.SectorId;
                }
            }

            // Mesma validação usada em ClienteEditar.aspx.cs (via ClientService): um NIF,
            // telefone ou código postal inválido é rejeitado aqui tal como seria no formulário manual.
            if (!_clientService.NifValido(nif, paisEhPortugal))
                throw new Exception(paisEhPortugal
                    ? "NIF inválido (deve ter 9 dígitos e dígito de controlo válido para Portugal)."
                    : "NIF inválido.");

            if (!_clientService.TelefoneValido(telefone, paisEhPortugal))
                throw new Exception("Telefone em formato inválido.");

            if (!_clientService.CodigoPostalValido(codigoPostal, paisEhPortugal))
                throw new Exception("Código postal inválido (formato esperado 1234-567 para Portugal).");

            var comercial = _userRepository.ListarComerciaisAtivos()
                .FirstOrDefault(u => u.Email.Equals(emailComercial, StringComparison.OrdinalIgnoreCase));

            if (comercial == null)
                throw new Exception($"Comercial com email '{emailComercial}' não encontrado, inativo ou sem perfil Comercial.");

            if (string.IsNullOrWhiteSpace(estado) || Array.IndexOf(EstadosValidos, estado) < 0)
                estado = "Potencial";

            return new Client
            {
                TradeName = nomeComercial,
                LegalName = string.IsNullOrWhiteSpace(nomeLegal) ? null : nomeLegal,
                VatNumber = nif,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                Phone = string.IsNullOrWhiteSpace(telefone) ? null : telefone,
                Address = string.IsNullOrWhiteSpace(morada) ? null : morada,
                PostalCode = string.IsNullOrWhiteSpace(codigoPostal) ? null : codigoPostal,
                City = string.IsNullOrWhiteSpace(cidade) ? null : cidade,
                CountryId = countryId,
                SectorId = sectorId,
                AccountManagerId = comercial.UserId,
                Status = estado,
                Notes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes,
                CreatedBy = UserId
            };
        }
    }
}