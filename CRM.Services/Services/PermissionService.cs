using System.Collections.Generic;
using System.Web;
using CRM.Data.Repositories;

namespace CRM.Services
{
    public enum NivelAcesso
    {
        Nenhum = 0,
        Consulta = 1,
        Proprios = 2,
        Total = 3
    }

    public class PermissionService
    {
        private readonly RoleRepository _roleRepository = new RoleRepository();
        private readonly RolePermissionRepository _rolePermissionRepository = new RolePermissionRepository();

        public NivelAcesso ObterNivel(string perfil, string modulo)
        {
            var codigos = ObterCodigosComCache(perfil);

            if (codigos.Contains($"{modulo}.Total")) return NivelAcesso.Total;
            if (codigos.Contains($"{modulo}.Proprios")) return NivelAcesso.Proprios;
            if (codigos.Contains($"{modulo}.Consulta")) return NivelAcesso.Consulta;
            return NivelAcesso.Nenhum;
        }

        private HashSet<string> ObterCodigosComCache(string perfil)
        {
            var sessao = HttpContext.Current?.Session;
            string chave = "PermissionCodes:" + perfil;

            if (sessao?[chave] is HashSet<string> emCache)
                return emCache;

            var role = _roleRepository.ObterPorNome(perfil);
            var codigos = role != null
                ? _rolePermissionRepository.ObterCodigosDoRole(role.RoleId)
                : new HashSet<string>();

            if (sessao != null)
                sessao[chave] = codigos;

            return codigos;
        }
    }
}