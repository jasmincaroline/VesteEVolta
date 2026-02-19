using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Controllers
{
    [TestFixture]
    public class ClothingsControllerTests
    {
        private PostgresContext _context = null!;
        private Mock<IClothingService> _clothingServiceMock = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _clothingServiceMock = new Mock<IClothingService>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private ClothingsController CreateControllerWithUser(Guid? userId)
        {
            var controller = new ClothingsController(_context, _clothingServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            if (userId.HasValue)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                    new Claim(ClaimTypes.Role, "Owner")
                };
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // sem claims
            }

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        private async Task SeedClothingAsync(Guid clothingId, Guid ownerId, string status = "AVAILABLE", decimal price = 100)
        {
            _context.TbClothings.Add(new TbClothing
            {
                Id = clothingId,
                Description = "Roupa teste",
                RentPrice = price,
                AvailabilityStatus = status,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        private async Task<(TbCategory cat1, TbCategory cat2)> SeedCategoriesAsync()
        {
            var c1 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Vestidos" };
            var c2 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Bolsas" };

            _context.TbCategories.AddRange(c1, c2);
            await _context.SaveChangesAsync();

            return (c1, c2);
        }

        // -------------------------
        // GET /clothes (JOIN + filtro)
        // -------------------------

        [Test]
        public async Task GetAll_WhenNoClothes_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            var controller = CreateControllerWithUser(userId: null);

            // Act
            var result = await controller.GetAll(null, null, null, null);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetAll_WhenFilterByStatus_ShouldReturnOnlyMatching()
        {
            // Arrange
            var controller = CreateControllerWithUser(null);

            await SeedClothingAsync(Guid.NewGuid(), Guid.NewGuid(), status: "AVAILABLE", price: 120);
            await SeedClothingAsync(Guid.NewGuid(), Guid.NewGuid(), status: "RENTED", price: 120);

            // Act
            var result = await controller.GetAll("AVAILABLE", null, null, null);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)result;

            // Como você retorna anonymous object, a gente valida pelo ToString/contagem via IEnumerable
            var list = ok.Value as System.Collections.IEnumerable;
            Assert.That(list, Is.Not.Null);

            // conta itens retornados
            var count = 0;
            foreach (var _ in list!) count++;
            Assert.That(count, Is.EqualTo(1));
        }

        // -------------------------
        // GET /clothes/{id}
        // -------------------------

        [Test]
        public async Task GetById_WhenNotFound_ShouldReturn404()
        {
            // Arrange
            var controller = CreateControllerWithUser(null);

            // Act
            var result = await controller.GetById(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        // -------------------------
        // PUT /clothes/{id}/categories
        // -------------------------

        [Test]
        public async Task UpdateCategories_WhenDtoNull_ShouldReturnBadRequest()
        {
            // Arrange
            var controller = CreateControllerWithUser(Guid.NewGuid());

            // Act
            var result = await controller.UpdateCategories(Guid.NewGuid(), null!);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateCategories_WhenCategoryIdsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var controller = CreateControllerWithUser(Guid.NewGuid());
            var dto = new ClothingCategoriesRequestDto { CategoryIds = new List<Guid>() };

            // Act
            var result = await controller.UpdateCategories(Guid.NewGuid(), dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateCategories_WhenClothingNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var controller = CreateControllerWithUser(Guid.NewGuid());
            var dto = new ClothingCategoriesRequestDto { CategoryIds = new List<Guid> { Guid.NewGuid() } };

            // Act
            var result = await controller.UpdateCategories(Guid.NewGuid(), dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateCategories_WhenSomeCategoryIdNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId);

            var (c1, _) = await SeedCategoriesAsync();

            // dto manda 1 id válido e 1 inválido
            var dto = new ClothingCategoriesRequestDto
            {
                CategoryIds = new List<Guid> { c1.CategoryId, Guid.NewGuid() }
            };

            // Act
            var result = await controller.UpdateCategories(clothingId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateCategories_WhenValid_ShouldReturnOk()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId);

            var (c1, c2) = await SeedCategoriesAsync();

            var dto = new ClothingCategoriesRequestDto
            {
                CategoryIds = new List<Guid> { c1.CategoryId, c2.CategoryId }
            };

            // Act
            var result = await controller.UpdateCategories(clothingId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            // confirma que salvou vínculo (se seu model tiver many-to-many configurado)
            var clothing = await _context.TbClothings.Include(x => x.Categories).FirstAsync(x => x.Id == clothingId);
            Assert.That(clothing.Categories.Count, Is.EqualTo(2));
        }

        // -------------------------
        // PUT /clothes/{id}
        // -------------------------

        [Test]
        public async Task Update_WhenTokenInvalid_ShouldReturnUnauthorized()
        {
            // Arrange
            var controller = CreateControllerWithUser(userId: null);
            var dto = new ClothingUpdateDto { Description = "ok", RentPrice = 10, AvailabilityStatus = "AVAILABLE" };

            // Act
            var result = await controller.Update(Guid.NewGuid(), dto);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Update_WhenClothingNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var dto = new ClothingUpdateDto { Description = "ok", RentPrice = 10, AvailabilityStatus = "AVAILABLE" };

            // Act
            var result = await controller.Update(Guid.NewGuid(), dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Update_WhenNotOwner_ShouldReturnForbid()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var controller = CreateControllerWithUser(otherUserId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId);

            var dto = new ClothingUpdateDto { Description = "ok", RentPrice = 10, AvailabilityStatus = "AVAILABLE" };

            // Act
            var result = await controller.Update(clothingId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task Update_WhenValid_ShouldReturnOkAndPersistChanges()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId, status: "AVAILABLE", price: 100);

            var dto = new ClothingUpdateDto
            {
                Description = "  Vestido novo  ",
                RentPrice = 250,
                AvailabilityStatus = "maintenance"
            };

            // Act
            var result = await controller.Update(clothingId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var updated = await _context.TbClothings.FirstAsync(x => x.Id == clothingId);
            Assert.That(updated.Description, Is.EqualTo("Vestido novo"));
            Assert.That(updated.RentPrice, Is.EqualTo(250));
            Assert.That(updated.AvailabilityStatus, Is.EqualTo("MAINTENANCE"));
        }

        // -------------------------
        // DELETE /clothes/{id}
        // -------------------------

        [Test]
        public async Task Delete_WhenTokenInvalid_ShouldReturnUnauthorized()
        {
            // Arrange
            var controller = CreateControllerWithUser(null);

            // Act
            var result = await controller.Delete(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Delete_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var controller = CreateControllerWithUser(Guid.NewGuid());

            // Act
            var result = await controller.Delete(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Delete_WhenNotOwner_ShouldReturnForbid()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var controller = CreateControllerWithUser(otherUserId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId, status: "AVAILABLE");

            // Act
            var result = await controller.Delete(clothingId);

            // Assert
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task Delete_WhenRented_ShouldReturnConflict()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId, status: "RENTED");

            // Act
            var result = await controller.Delete(clothingId);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task Delete_WhenValid_ShouldReturnNoContentAndRemove()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var clothingId = Guid.NewGuid();
            await SeedClothingAsync(clothingId, ownerId, status: "AVAILABLE");

            // Act
            var result = await controller.Delete(clothingId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(await _context.TbClothings.AnyAsync(x => x.Id == clothingId), Is.False);
        }

        // -------------------------
        // POST /clothes (usa service)
        // -------------------------

        [Test]
        public async Task Create_WhenServiceReturnsDto_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var controller = CreateControllerWithUser(ownerId);

            var dto = new CreateClothingDto { Description = "Vestido", RentPrice = 100 };

            _clothingServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateClothingDto>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new ClothingResponseDto
                {
                    Id = Guid.NewGuid(),
                    Description = "Vestido",
                    RentPrice = 100,
                    AvailabilityStatus = "AVAILABLE",
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            _clothingServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateClothingDto>(), It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
    }
}
