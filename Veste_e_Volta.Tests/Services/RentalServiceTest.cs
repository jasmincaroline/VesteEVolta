using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VesteEVolta.Models;
using VesteEVolta.Services;
using VesteEVolta.Repositories;

namespace VesteEVolta.Tests.Services
{
    [TestFixture]
    public class RentalServiceTests
    {
        private Mock<IRentalRepository> _rentalRepoMock = null!;
        private Mock<IClothingRepository> _clothingRepoMock = null!;
        private RentalService _rentalService = null!;
    
        [SetUp]
        public void Setup()
        {
            _rentalRepoMock = new Mock<IRentalRepository>();
            _clothingRepoMock = new Mock<IClothingRepository>();
            _rentalService = new RentalService(_rentalRepoMock.Object, _clothingRepoMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnRental_WhenExists()
        {
            var rentId = Guid.NewGuid();
            var rental = new TbRental { Id = rentId };

            _rentalRepoMock.Setup(r => r.GetById(rentId)).ReturnsAsync(rental);

            var result = await _rentalService.GetById(rentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(rentId));
        }

        [Test]
        public void GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            var rentId = Guid.NewGuid();
            _rentalRepoMock.Setup(r => r.GetById(rentId)).ReturnsAsync((TbRental?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _rentalService.GetById(rentId)
            );

            Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado."));
        }
    }
}
