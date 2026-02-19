using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private PostgresContext _context;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            // Usando InMemory database para testes
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _controller = new UsersController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAll Tests

        [Test]
        public async Task GetAll_UsersExist_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<TbUser>
            {
                new TbUser 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "João Silva", 
                    Email = "joao@test.com",
                    Telephone = "11999999999",
                    PasswordHash = "hash123",
                    ProfileType = "customer",
                    Reported = false,
                    CreatedAt = DateTime.UtcNow
                },
                new TbUser 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Maria Santos", 
                    Email = "maria@test.com",
                    Telephone = "11988888888",
                    PasswordHash = "hash456",
                    ProfileType = "owner",
                    Reported = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.TbUsers.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedUsers = result.Value as List<UserResponseDto>;
            Assert.That(returnedUsers, Is.Not.Null);
            Assert.That(returnedUsers.Count, Is.EqualTo(2));
            Assert.That(returnedUsers[0].Name, Is.EqualTo("João Silva"));
        }

        [Test]
        public async Task GetAll_NoUsers_ReturnsOkWithEmptyList()
        {
            // Arrange
            // Nenhum usuário no banco

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedUsers = result.Value as List<UserResponseDto>;
            Assert.That(returnedUsers, Is.Not.Null);
            Assert.That(returnedUsers.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_UserExists_ReturnsOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new TbUser
            {
                Id = userId,
                Name = "João Silva",
                Email = "joao@test.com",
                Telephone = "11999999999",
                PasswordHash = "hash123",
                ProfileType = "customer",
                Reported = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetById(userId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var returnedUser = result.Value as UserResponseDto;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.Id, Is.EqualTo(userId));
            Assert.That(returnedUser.Name, Is.EqualTo("João Silva"));
            Assert.That(returnedUser.Email, Is.EqualTo("joao@test.com"));
        }

        [Test]
        public async Task GetById_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.GetById(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Usuário não encontrado."));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ValidData_ReturnsOkWithUpdatedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new TbUser
            {
                Id = userId,
                Name = "João Silva",
                Email = "joao@test.com",
                Telephone = "11999999999",
                PasswordHash = "hash123",
                ProfileType = "customer",
                Reported = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            var updateDto = new UserUpdateDto
            {
                Name = "João Silva Updated",
                Email = "joao.updated@test.com",
                Telephone = "11988888888"
            };

            // Act
            var result = await _controller.Update(userId, updateDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var updatedUser = result.Value as UserResponseDto;
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser.Name, Is.EqualTo("João Silva Updated"));
            Assert.That(updatedUser.Email, Is.EqualTo("joao.updated@test.com"));
            Assert.That(updatedUser.Telephone, Is.EqualTo("11988888888"));
        }

        [Test]
        public async Task Update_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new UserUpdateDto
            {
                Name = "Test",
                Email = "test@test.com"
            };

            // Act
            var result = await _controller.Update(nonExistentId, updateDto) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Usuário não encontrado."));
        }

        [Test]
        public async Task Update_NullDto_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(userId, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Body inválido."));
        }

        [Test]
        public async Task Update_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UserUpdateDto
            {
                Name = "",
                Email = "test@test.com"
            };

            // Act
            var result = await _controller.Update(userId, updateDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Nome é obrigatório."));
        }

        [Test]
        public async Task Update_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UserUpdateDto
            {
                Name = "Test",
                Email = ""
            };

            // Act
            var result = await _controller.Update(userId, updateDto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Email é obrigatório."));
        }

        [Test]
        public async Task Update_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            var users = new List<TbUser>
            {
                new TbUser 
                { 
                    Id = user1Id,
                    Name = "User 1",
                    Email = "user1@test.com",
                    PasswordHash = "hash1",
                    ProfileType = "customer",
                    Reported = false,
                    CreatedAt = DateTime.UtcNow
                },
                new TbUser 
                { 
                    Id = user2Id,
                    Name = "User 2",
                    Email = "user2@test.com",
                    PasswordHash = "hash2",
                    ProfileType = "customer",
                    Reported = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.TbUsers.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var updateDto = new UserUpdateDto
            {
                Name = "User 1",
                Email = "user2@test.com" // Email já usado por user2
            };

            // Act
            var result = await _controller.Update(user1Id, updateDto) as ConflictObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Email já cadastrado por outro usuário."));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_UserExists_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new TbUser
            {
                Id = userId,
                Name = "João Silva",
                Email = "joao@test.com",
                PasswordHash = "hash123",
                ProfileType = "customer",
                Reported = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            
            var deletedUser = await _context.TbUsers.FindAsync(userId);
            Assert.That(deletedUser, Is.Null);
        }

        [Test]
        public async Task Delete_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(nonExistentId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Usuário não encontrado."));
        }

        #endregion
    }
}
