using System;
using VesteEVolta.Models;

namespace VesteEVolta.Validators
{
    public static class UserValidator
    {
        public static void ValidateUser(TbUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("Email é obrigatório.");

            if (!user.Email.Contains("@"))
                throw new InvalidOperationException("Email inválido.");
        }
    }
}
