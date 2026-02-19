using VesteEVolta.DTO;

namespace Veste_e_Volta.Tests.DTO;

[TestFixture]
public class AuthLoginDtoTests
{
    #region Property Assignment Tests

    [Test]
    public void AuthLoginDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "user@example.com",
            Password = "SecurePassword123"
        };

        // Assert
        Assert.That(dto.Email, Is.EqualTo("user@example.com"));
        Assert.That(dto.Password, Is.EqualTo("SecurePassword123"));
    }

    [Test]
    public void AuthLoginDto_EmptyEmail_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "",
            Password = "password"
        };

        // Assert
        Assert.That(dto.Email, Is.Empty);
        Assert.That(dto.Password, Is.EqualTo("password"));
    }

    [Test]
    public void AuthLoginDto_EmptyPassword_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "user@example.com",
            Password = ""
        };

        // Assert
        Assert.That(dto.Email, Is.EqualTo("user@example.com"));
        Assert.That(dto.Password, Is.Empty);
    }

    [Test]
    public void AuthLoginDto_WhitespaceEmail_AllowsWhitespace()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "   ",
            Password = "password"
        };

        // Assert
        Assert.That(dto.Email, Is.EqualTo("   "));
    }

    [Test]
    public void AuthLoginDto_LongPassword_SetsCorrectly()
    {
        // Arrange
        var longPassword = new string('a', 100);

        // Act
        var dto = new AuthLoginDto
        {
            Email = "test@test.com",
            Password = longPassword
        };

        // Assert
        Assert.That(dto.Password, Is.EqualTo(longPassword));
        Assert.That(dto.Password.Length, Is.EqualTo(100));
    }

    [Test]
    public void AuthLoginDto_SpecialCharactersInEmail_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "user+tag@example.co.uk",
            Password = "password"
        };

        // Assert
        Assert.That(dto.Email, Is.EqualTo("user+tag@example.co.uk"));
    }

    [Test]
    public void AuthLoginDto_SpecialCharactersInPassword_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthLoginDto
        {
            Email = "user@example.com",
            Password = "P@ssw0rd!#$%"
        };

        // Assert
        Assert.That(dto.Password, Is.EqualTo("P@ssw0rd!#$%"));
    }

    #endregion
}
