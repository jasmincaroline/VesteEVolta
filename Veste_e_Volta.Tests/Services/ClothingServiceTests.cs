using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Clothing
{
    [TestFixture]
    public class ClothingServiceTests
    {
        private PostgresContext _context;
        private ClothingService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _service = new ClothingService(_context);
        }

        private ClaimsPrincipal CreateOwnerUser(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Test]   //Teste feliz em que tudo dá certo
        public async Task CreateAsync_WhenValid_ShouldCreateClothing()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _context.TbOwners.Add(new TbOwner
            {
                UserId = userId
            });
            await _context.SaveChangesAsync();

            var dto = new CreateClothingDto
            {
                Description = " Vestido Azul ",
                RentPrice = 200
            };

            var user = CreateOwnerUser(userId);

            // Act
            var result = await _service.CreateAsync(dto, user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Description, Is.EqualTo("Vestido Azul"));
            Assert.That(result.AvailabilityStatus, Is.EqualTo("AVAILABLE"));
            Assert.That(result.OwnerId, Is.EqualTo(userId));

            Assert.That(_context.TbClothings.Count(), Is.EqualTo(1));
        }

        //DTO NULL
        [Test]
        public void CreateAsync_WhenDtoIsNull_ShouldThrow()
        {
            var user = CreateOwnerUser(Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(null!, user));
        }

        //Descrição vazia
        [Test]
        public void CreateAsync_WhenDescriptionIsEmpty_ShouldThrow()
        {
            var dto = new CreateClothingDto
            {
                Description = "",
                RentPrice = 100
            };

            var user = CreateOwnerUser(Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto, user));
        }

        //RentPrice <= 0
        [Test]
        public void CreateAsync_WhenRentPriceIsInvalid_ShouldThrow()
        {
            var dto = new CreateClothingDto
            {
                Description = "Vestido",
                RentPrice = 0
            };

            var user = CreateOwnerUser(Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto, user));
        }

        //Token inválido
        [Test]
        public void CreateAsync_WhenTokenInvalid_ShouldThrow()
        {
            var dto = new CreateClothingDto
            {
                Description = "Vestido",
                RentPrice = 100
            };

            var identity = new ClaimsIdentity(); // sem claim
            var user = new ClaimsPrincipal(identity);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateAsync(dto, user));
        }

        //Usuário sem perfil Owner
        [Test]
        public void CreateAsync_WhenUserIsNotOwner_ShouldThrow()
        {
            var userId = Guid.NewGuid();

            var dto = new CreateClothingDto
            {
                Description = "Vestido",
                RentPrice = 100
            };

            var user = CreateOwnerUser(userId);

            Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto, user));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
