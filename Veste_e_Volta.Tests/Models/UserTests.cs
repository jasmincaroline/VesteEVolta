using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Models;

[TestFixture]
public class UserTests
{
    #region Property Assignment Tests

    [Test]
    public void User_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 123,
            Name = "John Doe",
            Telephone = "+1234567890",
            Email = "john@example.com",
            Password = "hashedPassword123",
            Reported = false,
            ProfileType = "User"
        };

        // Assert
        Assert.That(user.Id, Is.EqualTo(123));
        Assert.That(user.Name, Is.EqualTo("John Doe"));
        Assert.That(user.Telephone, Is.EqualTo("+1234567890"));
        Assert.That(user.Email, Is.EqualTo("john@example.com"));
        Assert.That(user.Password, Is.EqualTo("hashedPassword123"));
        Assert.That(user.Reported, Is.False);
        Assert.That(user.ProfileType, Is.EqualTo("User"));
    }

    [Test]
    public void User_NullableProperties_AllowsNull()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 1,
            Name = null,
            Telephone = null,
            Email = null,
            Password = null,
            Reported = null,
            ProfileType = null
        };

        // Assert
        Assert.That(user.Id, Is.EqualTo(1));
        Assert.That(user.Name, Is.Null);
        Assert.That(user.Telephone, Is.Null);
        Assert.That(user.Email, Is.Null);
        Assert.That(user.Password, Is.Null);
        Assert.That(user.Reported, Is.Null);
        Assert.That(user.ProfileType, Is.Null);
    }

    [Test]
    public void User_ReportedTrue_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 456,
            Reported = true
        };

        // Assert
        Assert.That(user.Reported, Is.True);
    }

    [Test]
    public void User_ReportedFalse_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 789,
            Reported = false
        };

        // Assert
        Assert.That(user.Reported, Is.False);
    }

    [Test]
    public void User_OwnerProfileType_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 100,
            ProfileType = "Owner"
        };

        // Assert
        Assert.That(user.ProfileType, Is.EqualTo("Owner"));
    }

    [Test]
    public void User_UserProfileType_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 200,
            ProfileType = "User"
        };

        // Assert
        Assert.That(user.ProfileType, Is.EqualTo("User"));
    }

    [Test]
    public void User_EmptyStringProperties_AllowsEmpty()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 300,
            Name = "",
            Telephone = "",
            Email = "",
            Password = "",
            ProfileType = ""
        };

        // Assert
        Assert.That(user.Name, Is.Empty);
        Assert.That(user.Telephone, Is.Empty);
        Assert.That(user.Email, Is.Empty);
        Assert.That(user.Password, Is.Empty);
        Assert.That(user.ProfileType, Is.Empty);
    }

    [Test]
    public void User_ZeroId_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 0
        };

        // Assert
        Assert.That(user.Id, Is.EqualTo(0));
    }

    [Test]
    public void User_NegativeId_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = -1
        };

        // Assert
        Assert.That(user.Id, Is.EqualTo(-1));
    }

    [Test]
    public void User_LongName_SetsCorrectly()
    {
        // Arrange
        var longName = new string('a', 200);

        // Act
        var user = new User
        {
            Id = 400,
            Name = longName
        };

        // Assert
        Assert.That(user.Name, Is.EqualTo(longName));
        Assert.That(user.Name.Length, Is.EqualTo(200));
    }

    [Test]
    public void User_SpecialCharactersInName_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 500,
            Name = "José María O'Brien-Smith"
        };

        // Assert
        Assert.That(user.Name, Is.EqualTo("José María O'Brien-Smith"));
    }

    [Test]
    public void User_InternationalPhoneNumber_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 600,
            Telephone = "+55 11 98765-4321"
        };

        // Assert
        Assert.That(user.Telephone, Is.EqualTo("+55 11 98765-4321"));
    }

    [Test]
    public void User_EmailWithSpecialCharacters_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 700,
            Email = "user+tag@example.co.uk"
        };

        // Assert
        Assert.That(user.Email, Is.EqualTo("user+tag@example.co.uk"));
    }

    [Test]
    public void User_ComplexPassword_SetsCorrectly()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 800,
            Password = "$2a$10$abcdefghijklmnopqrstuv"
        };

        // Assert
        Assert.That(user.Password, Is.EqualTo("$2a$10$abcdefghijklmnopqrstuv"));
    }

    #endregion
}
