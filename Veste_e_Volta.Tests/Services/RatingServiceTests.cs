using Moq;
using NUnit.Framework;
using System.Text;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;

namespace Veste_e_Volta.Tests.Services;

[TestFixture]
public class RatingServiceTests
{
    private Mock<IRatingRepository> _mockRatingRepository = default!;
    private Mock<IRentalRepository> _mockRentalRepository = default!;
    private RatingService _service = default!;

    [SetUp]
    public void Setup()
    {
        _mockRatingRepository = new Mock<IRatingRepository>();
        _mockRentalRepository = new Mock<IRentalRepository>();
        _service = new RatingService(_mockRatingRepository.Object, _mockRentalRepository.Object);
    }

    [Test]
    public async Task GenerateReportAsync_NoRatings_ReturnsHeaderOnly()
    {
        // Arrange
        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<TbRating>());

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        Assert.That(csvContent, Does.Contain("Id,UserId,ClothingId,Score,Comment"));
        
        var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(1)); // Apenas o cabeçalho

        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_WithRatings_ReturnsCorrectCsvFormat()
    {
        // Arrange
        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Excelente roupa",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            },
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 4,
                Comment = "Muito bom",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(3)); // cabeçalho + 2 ratings
        
        Assert.That(lines[0], Is.EqualTo("Id,UserId,ClothingId,Score,Comment"));
        Assert.That(lines[1], Does.Contain(ratings[0].Id.ToString()));
        Assert.That(lines[1], Does.Contain(ratings[0].Rating.ToString()));
        Assert.That(lines[1], Does.Contain("Excelente roupa"));
        Assert.That(lines[2], Does.Contain(ratings[1].Id.ToString()));
        Assert.That(lines[2], Does.Contain("Muito bom"));

        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_NullComment_HandlesCorrectly()
    {
        // Arrange
        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 3,
                Comment = null,
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2));
        Assert.That(lines[1], Does.EndWith(",\"\""));

        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_CommentWithQuotes_EscapesCorrectly()
    {
        // Arrange
        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Roupa \"linda\" mesmo!",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        Assert.That(csvContent, Does.Contain("\"\"linda\"\""));
        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_MultipleRatingsWithVariousScores_ReturnsAllCorrectly()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var clothingId1 = Guid.NewGuid();
        var clothingId2 = Guid.NewGuid();

        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                ClothingId = clothingId1,
                Rating = 1,
                Comment = "Péssimo",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            },
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = userId2,
                ClothingId = clothingId2,
                Rating = 3,
                Comment = "Regular",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            },
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                ClothingId = clothingId2,
                Rating = 5,
                Comment = "Perfeito!",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(4)); // header + 3 ratings

        Assert.That(csvContent, Does.Contain(userId1.ToString()));
        Assert.That(csvContent, Does.Contain(userId2.ToString()));
        Assert.That(csvContent, Does.Contain(clothingId1.ToString()));
        Assert.That(csvContent, Does.Contain(clothingId2.ToString()));
        Assert.That(csvContent, Does.Contain("Péssimo"));
        Assert.That(csvContent, Does.Contain("Regular"));
        Assert.That(csvContent, Does.Contain("Perfeito!"));

        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_EmptyComment_HandlesCorrectly()
    {
        // Arrange
        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 4,
                Comment = "",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        var lines = csvContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines.Length, Is.EqualTo(2));

        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GenerateReportAsync_ReturnsUtf8EncodedBytes()
    {
        // Arrange
        var ratings = new List<TbRating>
        {
            new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Ótimo! Çedilha e acentuação",
                RentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRatingRepository
            .Setup(r => r.GetAll())
            .ReturnsAsync(ratings);

        // Act
        var result = await _service.GenerateReportAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        var csvContent = Encoding.UTF8.GetString(result);
        
        Assert.That(csvContent, Does.Contain("Ótimo! Çedilha e acentuação"));
        _mockRatingRepository.Verify(r => r.GetAll(), Times.Once);
    }
}
