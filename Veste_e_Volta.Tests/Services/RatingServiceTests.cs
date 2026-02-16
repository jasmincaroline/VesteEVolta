using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
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
            _rentalRepositoryMock.Object
        );
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenRentalNotFound()
    {
        // Arrange
        var dto = new RatingDto
        {
            RentId = Guid.NewGuid()
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(dto.RentId))
            .ReturnsAsync((TbRental?)null);

        // Act e Assert
        Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenUserIsNotRentalOwner()
    {
        // Arrange
        var rental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rental.Id,
            UserId = Guid.NewGuid()
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(dto.RentId))
            .ReturnsAsync(rental);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenRentalIsNotFinished()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var rental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Active"
        };

        var dto = new RatingDto
        {
            RentId = rental.Id,
            UserId = userId
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(dto.RentId))
            .ReturnsAsync(rental);

        // Act e Assert
        Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenRatingAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rentId = Guid.NewGuid();

        var rental = new TbRental
        {
            Id = rentId,
            UserId = userId,
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rentId,
            UserId = userId
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentId))
            .ReturnsAsync(rental);

        _ratingRepositoryMock
            .Setup(r => r.GetByRentIdAsync(rentId))
            .ReturnsAsync(new TbRating());

        // Act e Assert
        Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenClothingDoesNotMatchRental()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rentId = Guid.NewGuid();

        var rental = new TbRental
        {
            Id = rentId,
            UserId = userId,
            ClothingId = Guid.NewGuid(),
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rentId,
            UserId = userId,
            ClothingId = Guid.NewGuid()
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentId))
            .ReturnsAsync(rental);

        _ratingRepositoryMock
            .Setup(r => r.GetByRentIdAsync(rentId))
            .ReturnsAsync((TbRating?)null);

        // Act e Assert
        Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateRating_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rentId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();

        var rental = new TbRental
        {
            Id = rentId,
            UserId = userId,
            ClothingId = clothingId,
            Status = "Finished"
        };

        var dto = new RatingDto
        {
            RentId = rentId,
            UserId = userId,
            ClothingId = clothingId,
            Rating = 5,
            Comment = "Excelente!"
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentId))
            .ReturnsAsync(rental);

        _ratingRepositoryMock
            .Setup(r => r.GetByRentIdAsync(rentId))
            .ReturnsAsync((TbRating?)null);

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _ratingRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TbRating>()),
            Times.Once
        );
    }
}
