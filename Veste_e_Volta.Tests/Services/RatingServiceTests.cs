using Moq;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;

[TestFixture]
public class RatingServiceTests
{
    private Mock<IRatingRepository> _ratingRepositoryMock;
    private Mock<IRentalRepository> _rentalRepositoryMock;
    private RatingService _service;

    [SetUp]
    public void Setup()
    {
        _ratingRepositoryMock = new Mock<IRatingRepository>();
        _rentalRepositoryMock = new Mock<IRentalRepository>();

        _service = new RatingService(
            _ratingRepositoryMock.Object,
            _rentalRepositoryMock.Object);
    }

    [Test]
    public void CreateAsync_RentNotFound_ThrowsException()
    {
        var dto = new RatingDto { RentId = Guid.NewGuid() };

        _rentalRepositoryMock
            .Setup(r => r.GetById(dto.RentId))
            .ReturnsAsync((TbRental)null);

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.CreateAsync(dto));

        Assert.That(ex.Message, Is.EqualTo("Aluguel não encontrado."));
    }

    [Test]
    public void CreateAsync_UserMismatch_ThrowsException()
    {
        var rent = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rent.Id,
            UserId = Guid.NewGuid()
        };

        _rentalRepositoryMock.Setup(r => r.GetById(rent.Id)).ReturnsAsync(rent);

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.CreateAsync(dto));

        Assert.That(ex.Message, Is.EqualTo("Você não pode avaliar este aluguel."));
    }

    [Test]
    public void CreateAsync_StatusNotFinished_ThrowsException()
    {
        var rent = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Status = "Active"
        };

        var dto = new RatingDto
        {
            RentId = rent.Id,
            UserId = rent.UserId
        };

        _rentalRepositoryMock.Setup(r => r.GetById(rent.Id)).ReturnsAsync(rent);

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.CreateAsync(dto));

        Assert.That(ex.Message, Is.EqualTo("Só é possível avaliar após finalizar o aluguel."));
    }

    [Test]
    public async Task CreateAsync_ValidData_CallsAddAsync()
    {
        var rent = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rent.Id,
            UserId = rent.UserId,
            ClothingId = rent.ClothingId,
            Rating = 5,
            Comment = "Excelente!"
        };

        _rentalRepositoryMock.Setup(r => r.GetById(rent.Id)).ReturnsAsync(rent);
        _ratingRepositoryMock.Setup(r => r.GetByRentIdAsync(rent.Id)).ReturnsAsync((TbRating)null);

        await _service.CreateAsync(dto);

        _ratingRepositoryMock.Verify(r =>
            r.AddAsync(It.Is<TbRating>(x =>
                x.RentId == dto.RentId &&
                x.UserId == dto.UserId &&
                x.Rating == 5)),
            Times.Once);
    }

    [Test]
    public void DeleteAsync_NotFound_ThrowsException()
    {
        var id = Guid.NewGuid();

        _ratingRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TbRating)null);

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.DeleteAsync(id, Guid.NewGuid()));

        Assert.That(ex.Message, Is.EqualTo("Avaliação não encontrada."));
    }

    [Test]
    public void DeleteAsync_UserMismatch_ThrowsException()
    {
        var rating = new TbRating
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _ratingRepositoryMock.Setup(r => r.GetByIdAsync(rating.Id)).ReturnsAsync(rating);

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.DeleteAsync(rating.Id, Guid.NewGuid()));

        Assert.That(ex.Message, Is.EqualTo("Você não pode excluir essa avaliação."));
    }

    [Test]
    public async Task DeleteAsync_Valid_CallsDeleteAsync()
    {
        var rating = new TbRating
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _ratingRepositoryMock.Setup(r => r.GetByIdAsync(rating.Id)).ReturnsAsync(rating);

        await _service.DeleteAsync(rating.Id, rating.UserId);

        _ratingRepositoryMock.Verify(r => r.DeleteAsync(rating), Times.Once);
    }
}
