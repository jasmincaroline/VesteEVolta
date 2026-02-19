using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class ClothingsControllerTests
    {
        private PostgresContext _context;
        private Mock<IClothingService> _clothingServiceMock;
        private ClothingsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _clothingServiceMock = new Mock<IClothingService>();
            _controller = new ClothingsController(_context, _clothingServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SetupControllerUser(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("sub", userId.ToString()),
                new Claim(ClaimTypes.Role, "Owner")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = claimsPrincipal }
            };
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_NoFilters_ReturnsAllClothes()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new TbOwner 
            { 
                UserId = ownerId
            };

            var clothes = new List<TbClothing>
            {
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Vestido vermelho",
                    RentPrice = 50,
                    AvailabilityStatus = "AVAILABLE",
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                },
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Terno preto",
                    RentPrice = 100,
                    AvailabilityStatus = "RENTED",
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddRangeAsync(clothes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll(null, null, null, null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedClothes = result.Value as IEnumerable<object>;
            Assert.That(returnedClothes, Is.Not.Null);
            Assert.That(returnedClothes.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAll_FilterByStatus_ReturnsFilteredClothes()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new TbOwner 
            { 
                UserId = ownerId
            };

            var clothes = new List<TbClothing>
            {
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Item 1",
                    RentPrice = 50,
                    AvailabilityStatus = "AVAILABLE",
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                },
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Item 2",
                    RentPrice = 100,
                    AvailabilityStatus = "RENTED",
                    OwnerId = ownerId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddRangeAsync(clothes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll("AVAILABLE", null, null, null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedClothes = result.Value as IEnumerable<object>;
            Assert.That(returnedClothes, Is.Not.Null);
            Assert.That(returnedClothes.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAll_FilterByPriceRange_ReturnsFilteredClothes()
        {
            // Arrange
            var owner = new TbOwner 
            { 
                UserId = Guid.NewGuid()
            };

            var clothes = new List<TbClothing>
            {
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Cheap item",
                    RentPrice = 30,
                    AvailabilityStatus = "AVAILABLE",
                    OwnerId = owner.UserId,
                    CreatedAt = DateTime.UtcNow
                },
                new TbClothing 
                { 
                    Id = Guid.NewGuid(),
                    Description = "Expensive item",
                    RentPrice = 200,
                    AvailabilityStatus = "AVAILABLE",
                    OwnerId = owner.UserId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddRangeAsync(clothes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll(null, null, 50, 150) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedClothes = result.Value as IEnumerable<object>;
            Assert.That(returnedClothes, Is.Not.Null);
            Assert.That(returnedClothes.Count(), Is.EqualTo(0)); // Nenhum no range 50-150
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ClothingExists_ReturnsOkWithClothing()
        {
            // Arrange
            var owner = new TbOwner 
            { 
                UserId = Guid.NewGuid()
            };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Vestido elegante",
                RentPrice = 80,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = owner.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetById(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetById_ClothingNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.GetById(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Roupa não encontrada."));
        }

        #endregion

        #region GetCategories Tests

        [Test]
        public async Task GetCategories_ClothingWithCategories_ReturnsOkWithCategories()
        {
            // Arrange
            var owner = new TbOwner 
            { 
                UserId = Guid.NewGuid()
            };

            var category1 = new TbCategory
            {
                CategoryId = Guid.NewGuid(),
                Name = "Formal"
            };

            var category2 = new TbCategory
            {
                CategoryId = Guid.NewGuid(),
                Name = "Casual"
            };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa versátil",
                RentPrice = 60,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = owner.UserId,
                CreatedAt = DateTime.UtcNow,
                Categories = new List<TbCategory> { category1, category2 }
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbCategories.AddRangeAsync(category1, category2);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCategories(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var categories = result.Value as IEnumerable<object>;
            Assert.That(categories, Is.Not.Null);
            Assert.That(categories.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCategories_ClothingNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.GetCategories(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Roupa não encontrada."));
        }

        #endregion

        #region UpdateCategories Tests

        [Test]
        public async Task UpdateCategories_ValidData_ReturnsOkWithUpdatedCategories()
        {
            // Arrange
            var owner = new TbOwner 
            { 
                UserId = Guid.NewGuid()
            };

            var category1 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Formal" };
            var category2 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Casual" };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = owner.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbCategories.AddRangeAsync(category1, category2);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            var dto = new ClothingCategoriesRequestDto
            {
                CategoryIds = new List<Guid> { category1.CategoryId, category2.CategoryId }
            };

            // Act
            var result = await _controller.UpdateCategories(clothingId, dto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateCategories_NullDto_ReturnsBadRequest()
        {
            // Arrange
            var clothingId = Guid.NewGuid();

            // Act
            var result = await _controller.UpdateCategories(clothingId, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Body inválido."));
        }

        [Test]
        public async Task UpdateCategories_EmptyCategoryList_ReturnsBadRequest()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var dto = new ClothingCategoriesRequestDto
            {
                CategoryIds = new List<Guid>()
            };

            // Act
            var result = await _controller.UpdateCategories(clothingId, dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Informe ao menos uma categoria."));
        }

        [Test]
        public async Task UpdateCategories_ClothingNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var dto = new ClothingCategoriesRequestDto
            {
                CategoryIds = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            var result = await _controller.UpdateCategories(nonExistentId, dto) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Roupa não encontrada."));
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateClothingDto
            {
                Description = "Nova roupa",
                RentPrice = 75
            };

            var createdClothing = new ClothingResponseDto
            {
                Id = Guid.NewGuid(),
                Description = dto.Description,
                RentPrice = dto.RentPrice,
                AvailabilityStatus = "AVAILABLE"
            };

            _clothingServiceMock
                .Setup(s => s.CreateAsync(dto, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(createdClothing);

            // Act
            var result = await _controller.Create(dto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(ClothingsController.GetById)));
            Assert.That(result.Value, Is.EqualTo(createdClothing));
            _clothingServiceMock.Verify(s => s.CreateAsync(dto, It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Test]
        public void Create_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var dto = new CreateClothingDto
            {
                Description = "Nova roupa",
                RentPrice = 75
            };

            _clothingServiceMock
                .Setup(s => s.CreateAsync(dto, It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Owner not found"));

            // Act & Assert
            Assert.That(async () => await _controller.Create(dto),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Owner not found"));
            _clothingServiceMock.Verify(s => s.CreateAsync(dto, It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ValidData_ReturnsOkWithUpdatedClothing()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            SetupControllerUser(ownerId);
            
            var owner = new TbOwner { UserId = ownerId };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa antiga",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            var updateDto = new ClothingUpdateDto
            {
                Description = "Roupa atualizada",
                RentPrice = 75,
                AvailabilityStatus = "AVAILABLE"
            };

            // Act
            var result = await _controller.Update(clothingId, updateDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var updatedClothing = await _context.TbClothings.FindAsync(clothingId);
            Assert.That(updatedClothing!.Description, Is.EqualTo("Roupa atualizada"));
            Assert.That(updatedClothing.RentPrice, Is.EqualTo(75));
        }

        [Test]
        public async Task Update_NullDto_ReturnsBadRequest()
        {
            // Arrange
            var clothingId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(clothingId, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Body inválido."));
        }

        [Test]
        public async Task Update_EmptyDescription_ReturnsBadRequest()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var updateDto = new ClothingUpdateDto
            {
                Description = "",
                RentPrice = 75,
                AvailabilityStatus = "AVAILABLE"
            };

            // Act
            var result = await _controller.Update(clothingId, updateDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Descrição é obrigatória."));
        }

        [Test]
        public async Task Update_NegativePrice_ReturnsBadRequest()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var updateDto = new ClothingUpdateDto
            {
                Description = "Roupa",
                RentPrice = -10,
                AvailabilityStatus = "AVAILABLE"
            };

            // Act
            var result = await _controller.Update(clothingId, updateDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Valor do aluguel deve ser maior que zero."));
        }

        [Test]
        public async Task Update_ClothingNotFound_ReturnsNotFound()
        {
            // Arrange
            SetupControllerUser(Guid.NewGuid());
            var nonExistentId = Guid.NewGuid();
            var updateDto = new ClothingUpdateDto
            {
                Description = "Roupa",
                RentPrice = 75,
                AvailabilityStatus = "AVAILABLE"
            };

            // Act
            var result = await _controller.Update(nonExistentId, updateDto) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Roupa não encontrada."));
        }

        [Test]
        public async Task Update_NotOwner_ReturnsForbid()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            SetupControllerUser(ownerId);
            
            var owner = new TbOwner { UserId = ownerId };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = Guid.NewGuid(), // Different owner
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            var updateDto = new ClothingUpdateDto
            {
                Description = "Roupa atualizada",
                RentPrice = 75,
                AvailabilityStatus = "AVAILABLE"
            };

            // Act
            var result = await _controller.Update(clothingId, updateDto) as ForbidResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ValidClothing_ReturnsNoContent()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            SetupControllerUser(ownerId);
            
            var owner = new TbOwner { UserId = ownerId };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa para deletar",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(clothingId) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var deletedClothing = await _context.TbClothings.FindAsync(clothingId);
            Assert.That(deletedClothing, Is.Null);
        }

        [Test]
        public async Task Delete_ClothingNotFound_ReturnsNotFound()
        {
            // Arrange
            SetupControllerUser(Guid.NewGuid());
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Roupa não encontrada :(."));
        }

        [Test]
        public async Task Delete_NotOwner_ReturnsForbid()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            SetupControllerUser(ownerId);
            
            var owner = new TbOwner { UserId = ownerId };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = Guid.NewGuid(), // Different owner
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(clothingId) as ForbidResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Delete_RentedClothing_ReturnsConflict()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            SetupControllerUser(ownerId);
            
            var owner = new TbOwner { UserId = ownerId };

            var clothingId = Guid.NewGuid();
            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Roupa alugada",
                RentPrice = 50,
                AvailabilityStatus = "RENTED",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(clothingId) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Não é possível deletar uma roupa alugada (RENTED)."));
        }

        #endregion
    }
}
