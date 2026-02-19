using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VesteEVolta.Controllers;

namespace Veste_e_Volta.Tests.Controllers;

[TestFixture]
public class DebugAuthControllerTests
{
    private DebugAuthController _controller;

    [SetUp]
    public void Setup()
    {
        _controller = new DebugAuthController();
    }

    #region Me Tests

    [Test]
    public void Me_AuthenticatedUser_ReturnsOkWithClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim(ClaimTypes.Email, "john@example.com"),
            new Claim(ClaimTypes.Role, "User")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = _controller.Me();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedClaims = okResult.Value as IEnumerable<object>;
        Assert.That(returnedClaims, Is.Not.Null);
        Assert.That(returnedClaims.Count(), Is.EqualTo(4));
    }

    [Test]
    public void Me_UserWithMultipleClaims_ReturnsAllClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "456"),
            new Claim(ClaimTypes.Name, "Jane Smith"),
            new Claim(ClaimTypes.Email, "jane@example.com"),
            new Claim(ClaimTypes.Role, "Owner"),
            new Claim("CustomClaim", "CustomValue")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = _controller.Me();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedClaims = okResult.Value as IEnumerable<object>;
        Assert.That(returnedClaims, Is.Not.Null);
        Assert.That(returnedClaims.Count(), Is.EqualTo(5));
    }

    [Test]
    public void Me_UserWithNoClaims_ReturnsEmptyCollection()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = _controller.Me();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedClaims = okResult.Value as IEnumerable<object>;
        Assert.That(returnedClaims, Is.Not.Null);
        Assert.That(returnedClaims.Count(), Is.EqualTo(0));
    }

    [Test]
    public void Me_UserWithMinimalClaims_ReturnsOkWithOneClaim()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "789")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = _controller.Me();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedClaims = okResult.Value as IEnumerable<object>;
        Assert.That(returnedClaims, Is.Not.Null);
        Assert.That(returnedClaims.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Me_ReturnsOkResult_Success()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "test@test.com")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Act
        var result = _controller.Me();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    #endregion
}
