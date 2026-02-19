using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private PostgresContext _context;
    private AuthController _controller;
    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PostgresContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new PostgresContext(options);

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "ThisIsAVerySecureKeyForTestingPurposesOnly123456789"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _controller = new AuthController(_context, _configuration);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region Register Tests

    [Test]
    public void Register_ValidUserData_ReturnsCreated()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "John Doe",
            Telephone = "+1234567890",
            Email = "john@example.com",
            Password = "SecurePassword123",
            ProfileType = "User"
        };

        // Act
        var result = _controller.Register(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
        var createdResult = result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);

        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb!.Name, Is.EqualTo(dto.Name));
        Assert.That(userInDb.Email, Is.EqualTo(dto.Email));
        Assert.That(userInDb.ProfileType, Is.EqualTo(dto.ProfileType));
        Assert.That(userInDb.Reported, Is.False);
    }

    [Test]
    public void Register_OwnerProfileType_CreatesOwnerRecord()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "Jane Owner",
            Email = "owner@example.com",
            Password = "OwnerPass123",
            ProfileType = "Owner"
        };

        // Act
        var result = _controller.Register(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());

        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);

        var ownerInDb = _context.TbOwners.FirstOrDefault(o => o.UserId == userInDb!.Id);
        Assert.That(ownerInDb, Is.Not.Null);
    }

    [Test]
    public void Register_UserProfileType_DoesNotCreateOwnerRecord()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "Regular User",
            Email = "user@example.com",
            Password = "UserPass123",
            ProfileType = "User"
        };

        // Act
        var result = _controller.Register(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());

        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);

        var ownerInDb = _context.TbOwners.FirstOrDefault(o => o.UserId == userInDb!.Id);
        Assert.That(ownerInDb, Is.Null);
    }

    [Test]
    public void Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var existingUser = new TbUser
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "existing@example.com",
            PasswordHash = "hash",
            ProfileType = "User",
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.TbUsers.Add(existingUser);
        _context.SaveChanges();

        var dto = new AuthRegisterDto
        {
            Name = "New User",
            Email = "existing@example.com",
            Password = "password",
            ProfileType = "User"
        };

        // Act
        var result = _controller.Register(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        var conflictResult = result as ConflictObjectResult;
        Assert.That(conflictResult!.Value, Is.EqualTo("Esse email já está cadastrado."));
    }

    [Test]
    public void Register_HashesPassword_Correctly()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "PlainTextPassword",
            ProfileType = "User"
        };

        // Act
        _controller.Register(dto);

        // Assert
        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb!.PasswordHash, Is.Not.EqualTo(dto.Password));
        Assert.That(userInDb.PasswordHash, Is.Not.Empty);
    }

    [Test]
    public void Register_WithTelephone_SavesTelephoneCorrectly()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "User With Phone",
            Telephone = "+55 11 98765-4321",
            Email = "phone@example.com",
            Password = "password",
            ProfileType = "User"
        };

        // Act
        _controller.Register(dto);

        // Assert
        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb!.Telephone, Is.EqualTo(dto.Telephone));
    }

    [Test]
    public void Register_WithoutTelephone_SavesNullTelephone()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "User Without Phone",
            Telephone = null,
            Email = "nophone@example.com",
            Password = "password",
            ProfileType = "User"
        };

        // Act
        _controller.Register(dto);

        // Assert
        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb!.Telephone, Is.Null);
    }

    [Test]
    public void Register_SetsCreatedAtTimestamp_Correctly()
    {
        // Arrange
        var dto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "timestamp@example.com",
            Password = "password",
            ProfileType = "User"
        };

        var beforeRegistration = DateTime.UtcNow.AddSeconds(-1);

        // Act
        _controller.Register(dto);

        // Assert
        var afterRegistration = DateTime.UtcNow.AddSeconds(1);
        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb!.CreatedAt, Is.GreaterThanOrEqualTo(beforeRegistration));
        Assert.That(userInDb.CreatedAt, Is.LessThanOrEqualTo(afterRegistration));
    }

    #endregion

    #region Login Tests

    [Test]
    public void Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Login Test User",
            Email = "login@example.com",
            Password = "password123",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "login@example.com",
            Password = "password123"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var value = okResult!.Value;
        Assert.That(value, Is.Not.Null);

        var tokenProperty = value!.GetType().GetProperty("token");
        var token = tokenProperty!.GetValue(value) as string;
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public void Login_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new AuthLoginDto
        {
            Email = "nonexistent@example.com",
            Password = "password"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult!.Value, Is.EqualTo("Email ou senha inválidos."));
    }

    [Test]
    public void Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Test User",
            Email = "wrongpass@example.com",
            Password = "correctpassword",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "wrongpass@example.com",
            Password = "wrongpassword"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.That(unauthorizedResult!.Value, Is.EqualTo("Email ou senha inválidos."));
    }

    [Test]
    public void Login_ReturnsUserId_InResponse()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "User ID Test",
            Email = "userid@example.com",
            Password = "password",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "userid@example.com",
            Password = "password"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var value = okResult!.Value;
        var userIdProperty = value!.GetType().GetProperty("userId");
        var userId = (Guid)userIdProperty!.GetValue(value)!;

        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == loginDto.Email);
        Assert.That(userId, Is.EqualTo(userInDb!.Id));
    }

    [Test]
    public void Login_ReturnsProfileType_InResponse()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Profile Type Test",
            Email = "profiletype@example.com",
            Password = "password",
            ProfileType = "Owner"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "profiletype@example.com",
            Password = "password"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var value = okResult!.Value;
        var profileTypeProperty = value!.GetType().GetProperty("profileType");
        var profileType = profileTypeProperty!.GetValue(value) as string;

        Assert.That(profileType, Is.EqualTo("Owner"));
    }

    [Test]
    public void Login_UserProfileType_ReturnsCorrectProfileType()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Regular User",
            Email = "regularuser@example.com",
            Password = "password",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "regularuser@example.com",
            Password = "password"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        var okResult = result as OkObjectResult;
        var value = okResult!.Value;
        var profileTypeProperty = value!.GetType().GetProperty("profileType");
        var profileType = profileTypeProperty!.GetValue(value) as string;

        Assert.That(profileType, Is.EqualTo("User"));
    }

    [Test]
    public void Login_GeneratesJwtToken_WithCorrectFormat()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "JWT Test User",
            Email = "jwt@example.com",
            Password = "password",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "jwt@example.com",
            Password = "password"
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        var okResult = result as OkObjectResult;
        var value = okResult!.Value;
        var tokenProperty = value!.GetType().GetProperty("token");
        var token = tokenProperty!.GetValue(value) as string;

        // JWT tokens have 3 parts separated by dots
        var parts = token!.Split('.');
        Assert.That(parts.Length, Is.EqualTo(3));
    }

    [Test]
    public void Login_CaseSensitivePassword_ReturnsUnauthorized()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Case Sensitive Test",
            Email = "case@example.com",
            Password = "Password123",
            ProfileType = "User"
        };

        _controller.Register(registerDto);

        var loginDto = new AuthLoginDto
        {
            Email = "case@example.com",
            Password = "password123" // Different case
        };

        // Act
        var result = _controller.Login(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    #endregion

    #region Integration Tests

    [Test]
    public void RegisterAndLogin_FullFlow_WorksCorrectly()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Full Flow User",
            Telephone = "+1234567890",
            Email = "fullflow@example.com",
            Password = "SecurePassword123",
            ProfileType = "User"
        };

        // Act - Register
        var registerResult = _controller.Register(registerDto);

        // Assert - Register
        Assert.That(registerResult, Is.InstanceOf<CreatedResult>());

        // Act - Login
        var loginDto = new AuthLoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResult = _controller.Login(loginDto);

        // Assert - Login
        Assert.That(loginResult, Is.InstanceOf<OkObjectResult>());
        var okResult = loginResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        
        var value = okResult!.Value;
        var tokenProperty = value!.GetType().GetProperty("token");
        var token = tokenProperty!.GetValue(value) as string;
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public void RegisterOwnerAndLogin_FullFlow_WorksCorrectly()
    {
        // Arrange
        var registerDto = new AuthRegisterDto
        {
            Name = "Owner Full Flow",
            Email = "ownerflow@example.com",
            Password = "OwnerPassword123",
            ProfileType = "Owner"
        };

        // Act - Register
        var registerResult = _controller.Register(registerDto);

        // Assert - Register creates owner record
        var userInDb = _context.TbUsers.FirstOrDefault(u => u.Email == registerDto.Email);
        var ownerInDb = _context.TbOwners.FirstOrDefault(o => o.UserId == userInDb!.Id);
        Assert.That(ownerInDb, Is.Not.Null);

        // Act - Login
        var loginDto = new AuthLoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResult = _controller.Login(loginDto);

        // Assert - Login returns Owner profile type
        var okResult = loginResult as OkObjectResult;
        var value = okResult!.Value;
        var profileTypeProperty = value!.GetType().GetProperty("profileType");
        var profileType = profileTypeProperty!.GetValue(value) as string;
        Assert.That(profileType, Is.EqualTo("Owner"));
    }

    #endregion
}
