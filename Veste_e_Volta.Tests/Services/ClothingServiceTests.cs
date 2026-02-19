using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Services;

[TestFixture]
public class ClothingServiceTests
{
    private PostgresContext _context = default!;
    private ClothingService _service = default!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PostgresContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PostgresContext(options);
        _service = new ClothingService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_ValidData_ReturnsClothingResponseDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "Vestido Elegante",
            RentPrice = 150.00m
        };

        var owner = new TbOwner { UserId = userId };
        await _context.TbOwners.AddAsync(owner);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act
        var result = await _service.CreateAsync(dto, claimsPrincipal);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.EqualTo("Vestido Elegante"));
        Assert.That(result.RentPrice, Is.EqualTo(150.00m));
        Assert.That(result.AvailabilityStatus, Is.EqualTo("AVAILABLE"));
        Assert.That(result.OwnerId, Is.EqualTo(userId));
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));

        var savedClothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == result.Id);
        Assert.That(savedClothing, Is.Not.Null);
    }

    [Test]
    public void CreateAsync_NullDto_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(null!, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Body inválido"));
    }

    [Test]
    public void CreateAsync_EmptyDescription_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "",
            RentPrice = 100.00m
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Descrição é obrigatória"));
    }

    [Test]
    public void CreateAsync_WhitespaceDescription_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "   ",
            RentPrice = 100.00m
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Descrição é obrigatória"));
    }

    [Test]
    public void CreateAsync_ZeroRentPrice_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 0
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("RentPrice deve ser maior que zero"));
    }

    [Test]
    public void CreateAsync_NegativeRentPrice_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = -50.00m
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("RentPrice deve ser maior que zero"));
    }

    [Test]
    public void CreateAsync_InvalidUserId_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 100.00m
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Token inválido"));
    }

    [Test]
    public void CreateAsync_MissingUserIdClaim_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 100.00m
        };

        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Token inválido"));
    }

    [Test]
    public async Task CreateAsync_UserNotOwner_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 100.00m
        };

        // Não adiciona owner ao banco
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(dto, claimsPrincipal));

        Assert.That(ex!.Message, Does.Contain("Usuário não possui perfil Owner cadastrado"));
    }

    [Test]
    public async Task CreateAsync_DescriptionWithWhitespace_TrimsAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "  Vestido com Espaços  ",
            RentPrice = 200.00m
        };

        var owner = new TbOwner { UserId = userId };
        await _context.TbOwners.AddAsync(owner);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Act
        var result = await _service.CreateAsync(dto, claimsPrincipal);

        // Assert
        Assert.That(result.Description, Is.EqualTo("Vestido com Espaços"));
    }

    [Test]
    public async Task CreateAsync_ValidData_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 100.00m
        };

        var owner = new TbOwner { UserId = userId };
        await _context.TbOwners.AddAsync(owner);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = await _service.CreateAsync(dto, claimsPrincipal);

        // Assert
        var afterCreate = DateTime.UtcNow.AddSeconds(1);
        Assert.That(result.CreatedAt, Is.GreaterThan(beforeCreate));
        Assert.That(result.CreatedAt, Is.LessThan(afterCreate));
    }
}
