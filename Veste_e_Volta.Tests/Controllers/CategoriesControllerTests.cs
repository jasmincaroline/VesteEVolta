using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Controllers
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private PostgresContext _context = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        private CategoriesController CreateController()
            => new CategoriesController(_context);

        private async Task SeedCategoryAsync(Guid id, string name)
        {
            _context.TbCategories.Add(new TbCategory
            {
                CategoryId = id,
                Name = name
            });

            await _context.SaveChangesAsync();
        }

        // ---------- CREATE /categories ----------

        [Test]
        public async Task Create_WhenDtoNull_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var result = await controller.Create(null!);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Create_WhenNameEmpty_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var dto = new CategoryRequestDto { Name = "   " };

            var result = await controller.Create(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Create_WhenDuplicateName_ShouldReturnConflict()
        {
            var controller = CreateController();

            await SeedCategoryAsync(Guid.NewGuid(), "Vestidos");

            var dto = new CategoryRequestDto { Name = "  VESTIDOS  " };

            var result = await controller.Create(dto);

            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task Create_WhenValid_ShouldReturnCreatedAtAction()
        {
            var controller = CreateController();

            var dto = new CategoryRequestDto { Name = "Bolsas" };

            var result = await controller.Create(dto);

            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

            // confirma que salvou
            Assert.That(await _context.TbCategories.AnyAsync(c => c.Name == "Bolsas"), Is.True);
        }

        // ---------- UPDATE /categories/{id} ----------

        [Test]
        public async Task Update_WhenDtoNull_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var result = await controller.Update(Guid.NewGuid(), null!);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_WhenNotFound_ShouldReturnNotFound()
        {
            var controller = CreateController();

            var dto = new CategoryRequestDto { Name = "Nova" };

            var result = await controller.Update(Guid.NewGuid(), dto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Update_WhenDuplicateName_ShouldReturnConflict()
        {
            var controller = CreateController();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            await SeedCategoryAsync(Guid.NewGuid(), "Vestidos");
            var dto = new CategoryRequestDto { Name = "VESTIDOS" };
            var result = await controller.Create(dto);
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task Update_WhenValid_ShouldReturnOk_AndPersist()
        {
            var controller = CreateController();

            var id = Guid.NewGuid();
            await SeedCategoryAsync(id, "Antigo");

            var dto = new CategoryRequestDto { Name = "  Novo Nome " };

            var result = await controller.Update(id, dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var updated = await _context.TbCategories.FirstAsync(c => c.CategoryId == id);
            Assert.That(updated.Name, Is.EqualTo("Novo Nome"));
        }

        // ---------- DELETE /categories/{id} ----------

        [Test]
        public async Task Delete_WhenNotFound_ShouldReturnNotFound()
        {
            var controller = CreateController();

            var result = await controller.Delete(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Delete_WhenHasClothings_ShouldReturnConflict()
        {
            var controller = CreateController();

            var categoryId = Guid.NewGuid();
            var category = new TbCategory { CategoryId = categoryId, Name = "Vestidos" };

            // precisa ter uma roupa associada pra cair no Conflict
            var clothing = new TbClothing
            {
                Id = Guid.NewGuid(),
                Description = "Roupa",
                RentPrice = 10,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            category.Clothings.Add(clothing);

            _context.TbCategories.Add(category);
            _context.TbClothings.Add(clothing);
            await _context.SaveChangesAsync();

            var result = await controller.Delete(categoryId);

            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task Delete_WhenValid_ShouldReturnNoContent_AndRemove()
        {
            var controller = CreateController();

            var id = Guid.NewGuid();
            await SeedCategoryAsync(id, "Bolsas");

            var result = await controller.Delete(id);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(await _context.TbCategories.AnyAsync(c => c.CategoryId == id), Is.False);
        }
    }
}