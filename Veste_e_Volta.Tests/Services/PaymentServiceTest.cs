using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace VesteEVolta.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private PostgresContext? _context;
        private PaymentService? _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);

            var repository = new PaymentRepository(_context);
            _service = new PaymentService(repository, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task Create_Should_CreatePayment_WhenRentalExists()
        {
            // Arrange
            var rentalId = Guid.NewGuid();

            var rental = new TbRental
            {
                Id = rentalId,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context!.TbRentals.Add(rental);
            await _context.SaveChangesAsync();

            var dto = new CreatePaymentDto
            {
                Amount = 150,
                PaymentMethod = "credit",
                PaymentStatus = "paid"
            };

            // Act
            var result = await _service!.Create(rentalId, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(150));
            Assert.That(result.PaymentStatus, Is.EqualTo("paid"));

            var savedPayment = await _context.TbPayments
                .FirstOrDefaultAsync(p => p.RentalId == rentalId);

            Assert.That(savedPayment, Is.Not.Null);
        }
    }
}
