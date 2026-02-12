using NUnit.Framework;
using System;
using VesteEVolta.Models;
using VesteEVolta.Validators;

namespace Veste_e_Volta.Tests;

[TestFixture]
public class UserValidatorTests
{
    [Test]
    [Description("Deve lançar exceção quando o email for inválido")]
    public void ValidateUser_WhenEmailIsInvalid_ShouldThrowException()
    {
        // Arrange
        var user = new TbUser
        {
            Email = "emailinvalido"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            UserValidator.ValidateUser(user)
        );
    }
}
