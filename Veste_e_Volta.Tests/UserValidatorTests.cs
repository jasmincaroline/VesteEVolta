using NUnit.Framework;
using System;
using VesteEVolta.Models;
using VesteEVolta.Validators;

[TestFixture]
public class UserValidatorTests
{
    [Test]
    [Description("Deve lançar exceção quando o email for inválido")]
    public void ValidateUser_WhenEmailIsInvalid_ShouldThrowException()
    {
        var user = new TbUser
        {
            Email = "emailinvalido"
        };

        Assert.Throws<InvalidOperationException>(() =>
            UserValidator.ValidateUser(user)
        );
    }
}
