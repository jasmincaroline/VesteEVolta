using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Controllers;

[TestFixture]
public class ClothingsControllerUpdateTests
{
    private PostgresContext _context = default!;
    private Mock<IClothingService> _mockClothingService = default!;
    private ClothingsController _controller = default!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PostgresContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PostgresContext(options);
        _mockClothingService = new Mock<IClothingService>();
        _controller = new ClothingsController(_context, _mockClothingService.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Test]
    public async Task Update_ValidData_ReturnsOkWithUpdatedClothing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var clothing = new TbClothing
        {
            Id = clothingId,
            Description = "Vestido Azul",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido Azul Marinho",
            RentPrice = 150.00m,
            AvailabilityStatus = "RENTED"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var updatedClothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == clothingId);
        Assert.That(updatedClothing, Is.Not.Null);
        Assert.That(updatedClothing!.Description, Is.EqualTo("Vestido Azul Marinho"));
        Assert.That(updatedClothing.RentPrice, Is.EqualTo(150.00m));
        Assert.That(updatedClothing.AvailabilityStatus, Is.EqualTo("RENTED"));
    }

    [Test]
    public async Task Update_NullDto_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, null!);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("Body inválido"));
    }

    [Test]
    public async Task Update_EmptyDescription_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("Descrição é obrigatória"));
    }

    [Test]
    public async Task Update_WhitespaceDescription_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "   ",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("Descrição é obrigatória"));
    }

    [Test]
    public async Task Update_ZeroRentPrice_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 0,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("Valor do aluguel deve ser maior que zero"));
    }

    [Test]
    public async Task Update_NegativeRentPrice_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = -50.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("Valor do aluguel deve ser maior que zero"));
    }

    [Test]
    public async Task Update_EmptyAvailabilityStatus_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = ""
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest!.Value, Does.Contain("AvailabilityStatus é obrigatório"));
    }

    [Test]
    public async Task Update_ClothingNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound!.Value, Does.Contain("Roupa não encontrada"));
    }

    [Test]
    public async Task Update_UserIsNotOwner_ReturnsForbid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var clothing = new TbClothing
        {
            Id = clothingId,
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido Atualizado",
            RentPrice = 150.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(differentUserId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    [Test]
    public async Task Update_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var clothingId = Guid.NewGuid();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        // Não configura claims válidos
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        var unauthorized = result as UnauthorizedObjectResult;
        Assert.That(unauthorized!.Value, Does.Contain("Token inválido"));
    }

    [Test]
    public async Task Update_DescriptionWithWhitespace_TrimsAndUpdates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var clothing = new TbClothing
        {
            Id = clothingId,
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        var dto = new ClothingUpdateDto
        {
            Description = "  Vestido Elegante  ",
            RentPrice = 150.00m,
            AvailabilityStatus = "AVAILABLE"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var updatedClothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == clothingId);
        Assert.That(updatedClothing!.Description, Is.EqualTo("Vestido Elegante"));
    }

    [Test]
    public async Task Update_AvailabilityStatusLowercase_ConvertsToUppercase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var clothing = new TbClothing
        {
            Id = clothingId,
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "rented"
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var updatedClothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == clothingId);
        Assert.That(updatedClothing!.AvailabilityStatus, Is.EqualTo("RENTED"));
    }

    [Test]
    public async Task Update_AvailabilityStatusWithWhitespace_TrimsAndConvertsToUppercase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var clothing = new TbClothing
        {
            Id = clothingId,
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m,
            AvailabilityStatus = "  unavailable  "
        };

        SetupUserClaims(userId);

        // Act
        var result = await _controller.Update(clothingId, dto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var updatedClothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == clothingId);
        Assert.That(updatedClothing!.AvailabilityStatus, Is.EqualTo("UNAVAILABLE"));
    }
}
