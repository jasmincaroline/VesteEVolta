using NUnit.Framework;
using Moq;
using VesteEVolta.Services;
using VesteEVolta.Repositories;
using VesteEVolta.Models;
using VesteEVolta.DTO;

[TestFixture]
public class RentalServiceTest
{
    private Mock<IRentalRepository> repositoryMock;
    private RentalService service;

    [SetUp]
    public void Setup()
    {
        repositoryMock = new Mock<IRentalRepository>();
        service = new RentalService(repositoryMock.Object);
    }

    [Test]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoRentalsExist()
    {
        // ARRANGE
        repositoryMock
            .Setup(x => x.GetAll())
            .ReturnsAsync(new List<TbRental>());

        // ACT
        var result = await service.GetAll();

        // ASSERT
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ShouldReturnDTO_WhenRentalExists()
    {
        // ARRANGE
        var rental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };

        repositoryMock
            .Setup(x => x.GetById(rental.Id))
            .ReturnsAsync(rental);

        // ACT
        var result = await service.GetById(rental.Id);

        // ASSERT
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(rental.Id));
        Assert.That(result.UserId, Is.EqualTo(rental.UserId));
    }

    [Test]
    public void UpdateStatus_ShouldThrowException_WhenRentalDoesNotExist()
    {
        // ARRANGE
        var id = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetById(id))
            .ReturnsAsync((TbRental?)null);

        // ACT
        Assert.ThrowsAsync<Exception>(async () =>
            await service.UpdateStatus(id, "finished"));
    }
}
