using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VesteEVolta.Controllers;

namespace Veste_e_Volta.Tests.Controllers;

[TestFixture]
public class TestAuthControllerTests
{
    private TestAuthController _controller;

    [SetUp]
    public void Setup()
    {
        _controller = new TestAuthController();
    }

    #region AnyLogged Tests

    [Test]
    public void AnyLogged_AuthenticatedUser_ReturnsOkWithMessage()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "Test User")
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
        var result = _controller.AnyLogged();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo("Qualquer pessoa logada ✅"));
    }

    [Test]
    public void AnyLogged_UserRole_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "456"),
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
        var result = _controller.AnyLogged();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Qualquer pessoa logada ✅"));
    }

    [Test]
    public void AnyLogged_OwnerRole_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "789"),
            new Claim(ClaimTypes.Role, "Owner")
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
        var result = _controller.AnyLogged();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Qualquer pessoa logada ✅"));
    }

    #endregion

    #region OnlyOwner Tests

    [Test]
    public void OnlyOwner_UserWithOwnerRole_ReturnsOkWithMessage()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Role, "Owner")
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
        var result = _controller.OnlyOwner();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Somente Owner ✅"));
    }

    [Test]
    public void OnlyOwner_OwnerRoleWithMultipleClaims_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "456"),
            new Claim(ClaimTypes.Email, "owner@test.com"),
            new Claim(ClaimTypes.Role, "Owner"),
            new Claim(ClaimTypes.Name, "Owner User")
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
        var result = _controller.OnlyOwner();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Somente Owner ✅"));
    }

    #endregion

    #region OnlyUser Tests

    [Test]
    public void OnlyUser_UserWithUserRole_ReturnsOkWithMessage()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
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
        var result = _controller.OnlyUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Somente User ✅"));
    }

    [Test]
    public void OnlyUser_UserRoleWithMultipleClaims_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "789"),
            new Claim(ClaimTypes.Email, "user@test.com"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Name, "Regular User")
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
        var result = _controller.OnlyUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Somente User ✅"));
    }

    [Test]
    public void OnlyUser_UserRoleMinimalClaims_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
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
        var result = _controller.OnlyUser();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo("Somente User ✅"));
    }

    #endregion
}
