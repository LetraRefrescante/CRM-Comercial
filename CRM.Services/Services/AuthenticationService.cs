using System;
using CRM.Business.Security;
using CRM.Data.Repositories;
using CRM.Models.Entities.Seguranca;

namespace CRM.Services
{
    public enum LoginResult
    {
        Success,
        InvalidCredentials,
        AccountLocked,
        AccountInactive
    }

    public class AuthenticationResult
    {
        public LoginResult Result { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordResetTokenRepository _tokenRepository = new PasswordResetTokenRepository();
        private readonly AuditService _auditService = new AuditService();
        private const int MinutosValidadeToken = 30;

        // Valores por defeito; idealmente viriam da tabela Settings
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        public void SolicitarRecuperacaoPassword(string email)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null) return;

            _tokenRepository.InvalidarTokensAtivos(user.UserId);

            string token = Guid.NewGuid().ToString("N");

            _tokenRepository.Criar(new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                DataCriacao = DateTime.UtcNow,
                DataExpiracao = DateTime.UtcNow.AddMinutes(MinutosValidadeToken),
                Utilizado = false
            });

            _auditService.Registar(user.UserId, "PasswordResetRequested", "User", user.UserId.ToString());

            try
            {
                EnviarEmailRecuperacao(user.Email, token);
            }
            catch (NotImplementedException)
            {
                System.Diagnostics.Trace.TraceWarning(
                    $"Email de recuperação não enviado (serviço de email por implementar). Token gerado para o utilizador {user.UserId}.");
            }
        }
        public bool TokenValido(string token)
        {
            var registo = _tokenRepository.ObterPorToken(token);
            return registo != null && registo.EstaValido();
        }

        public bool RedefinirPasswordComToken(string token, string novaPassword)
        {
            var registo = _tokenRepository.ObterPorToken(token);
            if (registo == null || !registo.EstaValido()) return false;

            string salt = CRM.Business.Security.PasswordHasher.GenerateSalt();
            string hash = CRM.Business.Security.PasswordHasher.HashPassword(novaPassword, salt);

            _userRepository.AtualizarPassword(registo.UserId, hash, salt);
            _tokenRepository.MarcarComoUtilizado(registo.Id);

            _auditService.Registar(registo.UserId, "PasswordReset", "User", registo.UserId.ToString());

            return true;
        }

        public bool AlterarPassword(int userId, string passwordAtual, string novaPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) return false;

            bool passwordCorreta = CRM.Business.Security.PasswordHasher.VerifyPassword(
                passwordAtual, user.PasswordHash, user.PasswordSalt);

            if (!passwordCorreta) return false;

            string salt = PasswordHasher.GenerateSalt();
            string hash = PasswordHasher.HashPassword(novaPassword, salt);

            _userRepository.AtualizarPassword(userId, hash, salt);

            _auditService.Registar(userId, "PasswordChanged", "User", userId.ToString());

            return true;
        }

        private void EnviarEmailRecuperacao(string email, string token)
        {
            throw new NotImplementedException("Ligar ao serviço de email do projeto.");
        }
        public AuthenticationService()
        {
            _userRepository = new UserRepository();
        }

        public AuthenticationResult Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Result = LoginResult.InvalidCredentials,
                    ErrorMessage = "Email ou password inválidos."
                };
            }

            // Verifica se a conta está bloqueada
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Result = LoginResult.AccountLocked,
                    ErrorMessage = $"Conta bloqueada até {user.LockedUntil.Value:dd/MM/yyyy HH:mm}."
                };
            }

            // Verifica se a conta está ativa
            if (user.Status != "Ativo")
            {
                return new AuthenticationResult
                {
                    Result = LoginResult.AccountInactive,
                    ErrorMessage = "Conta inativa ou bloqueada. Contacte o administrador."
                };
            }

            // Verifica a password
            bool passwordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

            if (!passwordValid)
            {
                _userRepository.RegisterFailedLogin(user.UserId, MaxFailedAttempts, LockoutMinutes);

                return new AuthenticationResult
                {
                    Result = LoginResult.InvalidCredentials,
                    ErrorMessage = "Email ou password inválidos."
                };
            }

            // Login bem sucedido
            _userRepository.RegisterSuccessfulLogin(user.UserId);

            return new AuthenticationResult
            {
                Result = LoginResult.Success,
                User = user
            };
        }
    }
}