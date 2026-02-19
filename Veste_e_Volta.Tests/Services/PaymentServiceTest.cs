using Microsoft.EntityFrameworkCore;
using Moq;
using VesteEVolta.Models;

[TestFixture]
public class PaymentServiceTests
{
    private Mock<IPaymentRepository> _paymentRepositoryMock;
    private PostgresContext _context;
    private PaymentService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PostgresContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new PostgresContext(options);
        _paymentRepositoryMock = new Mock<IPaymentRepository>();

        _service = new PaymentService(
            _paymentRepositoryMock.Object,
            _context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private async Task<Guid> CreateValidRentalAsync()
    {
        var rentalId = Guid.NewGuid();

        var rental = new TbRental
        {
            Id = rentalId,
            Status = "ACTIVE"
        };

        _context.TbRentals.Add(rental);
        await _context.SaveChangesAsync();

        return rentalId;
    }

    [Test]
    public async Task GetByRentalId_ValidRentalId_ReturnsPaymentList()
    {
        var rentalId = Guid.NewGuid();
        var payments = new List<TbPayment>
        {
            new TbPayment { Id = Guid.NewGuid(), RentalId = rentalId }
        };

        _paymentRepositoryMock
            .Setup(r => r.GetByRentalId(rentalId))
            .ReturnsAsync(payments);

        var result = await _service.GetByRentalId(rentalId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First().RentalId, Is.EqualTo(rentalId));

        _paymentRepositoryMock.Verify(r => r.GetByRentalId(rentalId), Times.Once);
    }

    [Test]
    public async Task GetById_ExistingId_ReturnsPayment()
    {
        var id = Guid.NewGuid();
        var payment = new TbPayment { Id = id };

        _paymentRepositoryMock
            .Setup(r => r.GetById(id))
            .ReturnsAsync(payment);

        var result = await _service.GetById(id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(id));

        _paymentRepositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Test]
    public async Task Create_ValidData_ReturnsCreatedPayment()
    {
        var rentalId = await CreateValidRentalAsync();

        var dto = new CreatePaymentDto
        {
            PaymentMethod = "credit",
            Amount = 100,
            PaymentStatus = "PAID"
        };

        var result = await _service.Create(rentalId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.RentalId, Is.EqualTo(rentalId));
        Assert.That(result.Amount, Is.EqualTo(100));
        Assert.That(result.PaymentStatus, Is.EqualTo("paid"));
        Assert.That(result.PaymentDate, Is.Not.EqualTo(default(DateTime)));

        _paymentRepositoryMock.Verify(r =>
            r.Add(It.Is<TbPayment>(p =>
                p.RentalId == rentalId &&
                p.Amount == 100 &&
                p.PaymentStatus == "paid")),
            Times.Once);
    }

    [Test]
    public void Create_RentalNotFound_ThrowsException()
    {
        var rentalId = Guid.NewGuid();

        var dto = new CreatePaymentDto
        {
            PaymentMethod = "credit",
            Amount = 100,
            PaymentStatus = "paid"
        };

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.Create(rentalId, dto));

        Assert.That(ex.Message, Is.EqualTo("Aluguel não encontrado."));
        _paymentRepositoryMock.Verify(r => r.Add(It.IsAny<TbPayment>()), Times.Never);
    }

    [Test]
    public async Task Create_AmountLessOrEqualZero_ThrowsException()
    {
        var rentalId = await CreateValidRentalAsync();

        var dto = new CreatePaymentDto
        {
            PaymentMethod = "credit",
            Amount = 0,
            PaymentStatus = "paid"
        };

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.Create(rentalId, dto));

        Assert.That(ex.Message, Is.EqualTo("O valor deve ser maior que 0."));
        _paymentRepositoryMock.Verify(r => r.Add(It.IsAny<TbPayment>()), Times.Never);
    }

    [Test]
    public async Task Create_InvalidStatus_ThrowsException()
    {
        var rentalId = await CreateValidRentalAsync();

        var dto = new CreatePaymentDto
        {
            PaymentMethod = "credit",
            Amount = 100,
            PaymentStatus = "invalid"
        };

        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _service.Create(rentalId, dto));

        Assert.That(ex.Message, Is.EqualTo("Status inválido."));
        _paymentRepositoryMock.Verify(r => r.Add(It.IsAny<TbPayment>()), Times.Never);
    }
}
