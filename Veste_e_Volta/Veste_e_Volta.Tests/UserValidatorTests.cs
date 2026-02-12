using NUnit.Framework;
using System;
using VesteEVolta.Models;
using VesteEVolta.Validators;

[TestFixture]
public class UserValidatorTest
{
    [Test]
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
