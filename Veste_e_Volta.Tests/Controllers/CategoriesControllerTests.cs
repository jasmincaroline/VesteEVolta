using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private PostgresContext _context = null!;
        private CategoriesController _controller = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _controller = new CategoriesController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<TbCategory>
            {
                new TbCategory { CategoryId = Guid.NewGuid(), Name = "Formal" },
                new TbCategory { CategoryId = Guid.NewGuid(), Name = "Casual" },
                new TbCategory { CategoryId = Guid.NewGuid(), Name = "Esportivo" }
            };

            await _context.TbCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedCategories = result.Value as List<TbCategory>;
            Assert.That(returnedCategories, Is.Not.Null);
            Assert.That(returnedCategories.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAll_NoCategories_ReturnsEmptyList()
        {
            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedCategories = result.Value as List<TbCategory>;
            Assert.That(returnedCategories, Is.Not.Null);
            Assert.That(returnedCategories.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_CategoryExists_ReturnsOkWithCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new TbCategory
            {
                CategoryId = categoryId,
                Name = "Formal"
            };

            await _context.TbCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetById(categoryId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedCategory = result.Value as TbCategory;
            Assert.That(returnedCategory, Is.Not.Null);
            Assert.That(returnedCategory.CategoryId, Is.EqualTo(categoryId));
            Assert.That(returnedCategory.Name, Is.EqualTo("Formal"));
        }

        [Test]
        public async Task GetById_CategoryNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.GetById(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Categoria não encontrada :("));
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CategoryRequestDto
            {
                Name = "Nova Categoria"
            };

            // Act
            var result = await _controller.Create(dto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(CategoriesController.GetById)));
            var createdCategory = result.Value as TbCategory;
            Assert.That(createdCategory, Is.Not.Null);
            Assert.That(createdCategory.Name, Is.EqualTo("Nova Categoria"));

            var savedCategory = await _context.TbCategories.FindAsync(createdCategory.CategoryId);
            Assert.That(savedCategory, Is.Not.Null);
        }

        [Test]
        public async Task Create_NullDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Create(null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Dados da categoria são obrigatórios."));
        }

        [Test]
        public async Task Create_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CategoryRequestDto
            {
                Name = ""
            };

            // Act
            var result = await _controller.Create(dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Nome é obrigatório, não esquece :)"));
        }

        [Test]
        public async Task Create_DuplicateName_ReturnsConflict()
        {
            // Arrange
            var existingCategory = new TbCategory
            {
                CategoryId = Guid.NewGuid(),
                Name = "Formal"
            };
            await _context.TbCategories.AddAsync(existingCategory);
            await _context.SaveChangesAsync();

            var dto = new CategoryRequestDto
            {
                Name = "formal" // Case insensitive
            };

            // Act
            var result = await _controller.Create(dto) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Já existe uma categoria com esse nome, escolha outro nome, por favor :)"));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ValidData_ReturnsOkWithUpdatedCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new TbCategory
            {
                CategoryId = categoryId,
                Name = "Nome Antigo"
            };

            await _context.TbCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            var updateDto = new CategoryRequestDto
            {
                Name = "Nome Atualizado"
            };

            // Act
            var result = await _controller.Update(categoryId, updateDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var updatedCategory = result.Value as TbCategory;
            Assert.That(updatedCategory, Is.Not.Null);
            Assert.That(updatedCategory.Name, Is.EqualTo("Nome Atualizado"));

            var savedCategory = await _context.TbCategories.FindAsync(categoryId);
            Assert.That(savedCategory!.Name, Is.EqualTo("Nome Atualizado"));
        }

        [Test]
        public async Task Update_NullDto_ReturnsBadRequest()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(categoryId, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Dados da categoria são obrigatórios."));
        }

        [Test]
        public async Task Update_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryRequestDto
            {
                Name = ""
            };

            // Act
            var result = await _controller.Update(categoryId, updateDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Nome é obrigatório, não esquece :)"));
        }

        [Test]
        public async Task Update_CategoryNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new CategoryRequestDto
            {
                Name = "Nome Novo"
            };

            // Act
            var result = await _controller.Update(nonExistentId, updateDto) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Categoria não encontrada :("));
        }

        [Test]
        public async Task Update_DuplicateName_ReturnsConflict()
        {
            // Arrange
            var category1 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Formal" };
            var category2 = new TbCategory { CategoryId = Guid.NewGuid(), Name = "Casual" };

            await _context.TbCategories.AddRangeAsync(category1, category2);
            await _context.SaveChangesAsync();

            var updateDto = new CategoryRequestDto
            {
                Name = "Formal" // Trying to rename category2 to same name as category1
            };

            // Act
            var result = await _controller.Update(category2.CategoryId, updateDto) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Já existe uma categoria com esse nome, escolha outro nome, por favor :)"));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ValidCategory_ReturnsNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new TbCategory
            {
                CategoryId = categoryId,
                Name = "Categoria para deletar"
            };

            await _context.TbCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(categoryId) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var deletedCategory = await _context.TbCategories.FindAsync(categoryId);
            Assert.That(deletedCategory, Is.Null);
        }

        [Test]
        public async Task Delete_CategoryNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Categoria não encontrada :("));
        }

        [Test]
        public async Task Delete_CategoryWithClothings_ReturnsConflict()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new TbOwner { UserId = ownerId };

            var categoryId = Guid.NewGuid();
            var category = new TbCategory
            {
                CategoryId = categoryId,
                Name = "Categoria com roupas"
            };

            var clothing = new TbClothing
            {
                Id = Guid.NewGuid(),
                Description = "Roupa",
                RentPrice = 50,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                Categories = new List<TbCategory> { category }
            };

            await _context.TbOwners.AddAsync(owner);
            await _context.TbCategories.AddAsync(category);
            await _context.TbClothings.AddAsync(clothing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(categoryId) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Não é possível deletar, pois a categoria está associada a uma roupa."));
        }

        #endregion
    }
}
