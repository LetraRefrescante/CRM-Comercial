using System;
using CRM.Business.Security;
using CRM.Data.Repositories;
using CRM.Models.Entities;

namespace CRM.Business.Services
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

        // Valores por defeito; idealmente viriam da tabela Settings
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        public AuthenticationService()
        {
            _userRepository = new UserRepository();
        }

        public AuthenticationResult Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            // Não revelar se o email existe ou não (mensagem genérica)
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