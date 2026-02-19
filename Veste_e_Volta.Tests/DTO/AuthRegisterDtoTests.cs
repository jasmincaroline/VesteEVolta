using VesteEVolta.DTO;

namespace Veste_e_Volta.Tests.DTO;

[TestFixture]
public class AuthRegisterDtoTests
{
    #region Property Assignment Tests

    [Test]
    public void AuthRegisterDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "John Doe",
            Telephone = "+1234567890",
            Email = "john@example.com",
            Password = "SecurePassword123",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo("John Doe"));
        Assert.That(dto.Telephone, Is.EqualTo("+1234567890"));
        Assert.That(dto.Email, Is.EqualTo("john@example.com"));
        Assert.That(dto.Password, Is.EqualTo("SecurePassword123"));
        Assert.That(dto.ProfileType, Is.EqualTo("User"));
    }

    [Test]
    public void AuthRegisterDto_NullTelephone_AllowsNull()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Jane Smith",
            Telephone = null,
            Email = "jane@example.com",
            Password = "password",
            ProfileType = "Owner"
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo("Jane Smith"));
        Assert.That(dto.Telephone, Is.Null);
        Assert.That(dto.Email, Is.EqualTo("jane@example.com"));
        Assert.That(dto.ProfileType, Is.EqualTo("Owner"));
    }

    [Test]
    public void AuthRegisterDto_EmptyName_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "",
            Email = "test@test.com",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Name, Is.Empty);
    }

    [Test]
    public void AuthRegisterDto_EmptyEmail_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Email, Is.Empty);
    }

    [Test]
    public void AuthRegisterDto_EmptyPassword_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "test@test.com",
            Password = "",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Password, Is.Empty);
    }

    [Test]
    public void AuthRegisterDto_EmptyProfileType_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "test@test.com",
            Password = "password",
            ProfileType = ""
        };

        // Assert
        Assert.That(dto.ProfileType, Is.Empty);
    }

    [Test]
    public void AuthRegisterDto_OwnerProfileType_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Owner Name",
            Email = "owner@test.com",
            Password = "password",
            ProfileType = "Owner"
        };

        // Assert
        Assert.That(dto.ProfileType, Is.EqualTo("Owner"));
    }

    [Test]
    public void AuthRegisterDto_UserProfileType_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "User Name",
            Email = "user@test.com",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.ProfileType, Is.EqualTo("User"));
    }

    [Test]
    public void AuthRegisterDto_LongName_SetsCorrectly()
    {
        // Arrange
        var longName = new string('a', 100);

        // Act
        var dto = new AuthRegisterDto
        {
            Name = longName,
            Email = "test@test.com",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo(longName));
        Assert.That(dto.Name.Length, Is.EqualTo(100));
    }

    [Test]
    public void AuthRegisterDto_SpecialCharactersInName_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "José María O'Brien-Smith",
            Email = "jose@test.com",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo("José María O'Brien-Smith"));
    }

    [Test]
    public void AuthRegisterDto_InternationalPhoneNumber_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Telephone = "+55 11 98765-4321",
            Email = "test@test.com",
            Password = "password",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Telephone, Is.EqualTo("+55 11 98765-4321"));
    }

    [Test]
    public void AuthRegisterDto_ComplexPassword_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "test@test.com",
            Password = "P@ssw0rd!#$%^&*()_+",
            ProfileType = "User"
        };

        // Assert
        Assert.That(dto.Password, Is.EqualTo("P@ssw0rd!#$%^&*()_+"));
    }

    [Test]
    public void AuthRegisterDto_AllPropertiesNull_AllowsConfiguration()
    {
        // Arrange & Act
        var dto = new AuthRegisterDto
        {
            Name = null!,
            Telephone = null,
            Email = null!,
            Password = null!,
            ProfileType = null!
        };

        // Assert
        Assert.That(dto.Name, Is.Null);
        Assert.That(dto.Telephone, Is.Null);
        Assert.That(dto.Email, Is.Null);
        Assert.That(dto.Password, Is.Null);
        Assert.That(dto.ProfileType, Is.Null);
    }

    #endregion
}
