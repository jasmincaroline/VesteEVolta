using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Models;

[TestFixture]
public class TbCustomerTests
{
    #region Property Assignment Tests

    [Test]
    public void TbCustomer_ValidGuid_SetsUserIdCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var customer = new TbCustomer
        {
            UserId = userId
        };

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(userId));
    }

    [Test]
    public void TbCustomer_EmptyGuid_SetsCorrectly()
    {
        // Arrange & Act
        var customer = new TbCustomer
        {
            UserId = Guid.Empty
        };

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void TbCustomer_WithUser_SetsNavigationPropertyCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hashedPassword",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(userId));
        Assert.That(customer.User, Is.Not.Null);
        Assert.That(customer.User.Id, Is.EqualTo(userId));
        Assert.That(customer.User.Name, Is.EqualTo("John Doe"));
        Assert.That(customer.User.Email, Is.EqualTo("john@example.com"));
    }

    [Test]
    public void TbCustomer_UserIdMatchesUser_IsConsistent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "Jane Smith",
            Email = "jane@example.com",
            PasswordHash = "hash123",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(customer.User.Id));
    }

    [Test]
    public void TbCustomer_MultipleInstances_MaintainSeparateIds()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var customer1 = new TbCustomer { UserId = userId1 };
        var customer2 = new TbCustomer { UserId = userId2 };

        // Assert
        Assert.That(customer1.UserId, Is.Not.EqualTo(customer2.UserId));
        Assert.That(customer1.UserId, Is.EqualTo(userId1));
        Assert.That(customer2.UserId, Is.EqualTo(userId2));
    }

    [Test]
    public void TbCustomer_NewInstance_UserIdIsDefault()
    {
        // Arrange & Act
        var customer = new TbCustomer();

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void TbCustomer_ChangingUserId_UpdatesCorrectly()
    {
        // Arrange
        var originalUserId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var customer = new TbCustomer { UserId = originalUserId };

        // Act
        customer.UserId = newUserId;

        // Assert
        Assert.That(customer.UserId, Is.EqualTo(newUserId));
        Assert.That(customer.UserId, Is.Not.EqualTo(originalUserId));
    }

    [Test]
    public void TbCustomer_UserWithReported_PreservesUserProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "Reported User",
            Email = "reported@example.com",
            PasswordHash = "hash",
            ProfileType = "User",
            Reported = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.User.Reported, Is.True);
        Assert.That(customer.User.Name, Is.EqualTo("Reported User"));
    }

    [Test]
    public void TbCustomer_UserWithTelephone_PreservesTelephone()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "User With Phone",
            Telephone = "+55 11 98765-4321",
            Email = "phone@example.com",
            PasswordHash = "hash",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.User.Telephone, Is.EqualTo("+55 11 98765-4321"));
        Assert.That(customer.User.Telephone, Is.Not.Null);
    }

    [Test]
    public void TbCustomer_UserWithoutTelephone_TelephoneIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "User Without Phone",
            Telephone = null,
            Email = "nophone@example.com",
            PasswordHash = "hash",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.User.Telephone, Is.Null);
    }

    [Test]
    public void TbCustomer_ProfileTypeUser_SetsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new TbUser
        {
            Id = userId,
            Name = "Regular User",
            Email = "regular@example.com",
            PasswordHash = "hash",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var customer = new TbCustomer
        {
            UserId = userId,
            User = user
        };

        // Assert
        Assert.That(customer.User.ProfileType, Is.EqualTo("User"));
    }

    #endregion
}
