using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
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

        private UsersController CreateController()
            => new UsersController(_context);

        private async Task SeedUserAsync(Guid id, string name, string email)
        {
            _context.TbUsers.Add(new TbUser
            {
                Id = id,
                Name = name,
                Telephone = "99999-9999",
                Email = email,
                ProfileType = "User",
                Reported = false,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "x"
            });

            await _context.SaveChangesAsync();
        }

        // ---------- UPDATE /users/{id} ----------

        [Test]
        public async Task Update_WhenDtoNull_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var result = await controller.Update(Guid.NewGuid(), null!);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_WhenNameEmpty_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var dto = new UserUpdateDto
            {
                Name = "   ",
                Email = "a@a.com",
                Telephone = "123"
            };

            var result = await controller.Update(Guid.NewGuid(), dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_WhenUserNotFound_ShouldReturnNotFound()
        {
            var controller = CreateController();

            var dto = new UserUpdateDto
            {
                Name = "Novo Nome",
                Email = "novo@email.com",
                Telephone = "123"
            };

            var result = await controller.Update(Guid.NewGuid(), dto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Update_WhenEmailDuplicate_ShouldReturnConflict()
        {
            var controller = CreateController();

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            await SeedUserAsync(userId1, "User 1", "teste@email.com");
            await SeedUserAsync(userId2, "User 2", "outro@email.com");

            // tenta atualizar user2 pra email do user1
            var dto = new UserUpdateDto
            {
                Name = "User 2 Novo",
                Email = "TESTE@EMAIL.COM", // case diferente pra validar ToLower()
                Telephone = "999"
            };

            var result = await controller.Update(userId2, dto);

            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

        [Test]
        public async Task Update_WhenValid_ShouldReturnOk_AndPersist()
        {
            var controller = CreateController();

            var userId = Guid.NewGuid();
            await SeedUserAsync(userId, "Nome Antigo", "antigo@email.com");

            var dto = new UserUpdateDto
            {
                Name = "  Nome Novo  ",
                Email = "  NOVO@EMAIL.COM ",
                Telephone = "  88888-8888  "
            };

            var result = await controller.Update(userId, dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var updated = await _context.TbUsers.FirstAsync(u => u.Id == userId);
            Assert.That(updated.Name, Is.EqualTo("Nome Novo"));
            Assert.That(updated.Email, Is.EqualTo("novo@email.com"));
            Assert.That(updated.Telephone, Is.EqualTo("88888-8888"));
        }

        [Test]
        public async Task Update_WhenTelephoneEmpty_ShouldPersistNullTelephone()
        {
            var controller = CreateController();

            var userId = Guid.NewGuid();
            await SeedUserAsync(userId, "Nome", "email@email.com");

            var dto = new UserUpdateDto
            {
                Name = "Nome",
                Email = "email@email.com",
                Telephone = "   "
            };

            var result = await controller.Update(userId, dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var updated = await _context.TbUsers.FirstAsync(u => u.Id == userId);
            Assert.That(updated.Telephone, Is.Null);
        }

        [Test]
        public async Task Update_WhenEmailEmpty_ShouldReturnBadRequest()
        {
            var controller = CreateController();

            var dto = new UserUpdateDto
            {
                Name = "Nome",
                Email = "   ",
                Telephone = "123"
            };

            var result = await controller.Update(Guid.NewGuid(), dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }


        // ---------- GET /users/{id} ----------

        [Test]
        public async Task GetById_WhenNotFound_ShouldReturnNotFound()
        {
            var controller = CreateController();

            var result = await controller.GetById(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAll_ShouldReturnOk_WithList()
        {
            var controller = CreateController();

            await SeedUserAsync(Guid.NewGuid(), "A", "a@a.com");
            await SeedUserAsync(Guid.NewGuid(), "B", "b@b.com");

            var result = await controller.GetAll();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult)result;
            Assert.That(ok.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetById_WhenFound_ShouldReturnOk()
        {
            var controller = CreateController();

            var userId = Guid.NewGuid();
            await SeedUserAsync(userId, "Maria", "maria@email.com");

            var result = await controller.GetById(userId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }



        // ---------- DELETE /users/{id} ----------

        [Test]
        public async Task Delete_WhenNotFound_ShouldReturnNotFound()
        {
            var controller = CreateController();

            var result = await controller.Delete(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Delete_WhenValid_ShouldReturnNoContent_AndRemove()
        {
            var controller = CreateController();

            var userId = Guid.NewGuid();
            await SeedUserAsync(userId, "Nome", "nome@email.com");

            var result = await controller.Delete(userId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(await _context.TbUsers.AnyAsync(u => u.Id == userId), Is.False);
        }

    }
}
