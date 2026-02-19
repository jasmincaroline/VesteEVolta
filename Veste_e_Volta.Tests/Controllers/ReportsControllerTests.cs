using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Text;
using VesteEVolta.Controllers;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Reports
{
    public class ReportsControllerTests
    {
        private PostgresContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new PostgresContext(options);
        }

        private ReportsController CreateController(PostgresContext ctx, ClaimsPrincipal? user = null)
        {
            var serviceMock = new Mock<IReportService>();

            var controller = new ReportsController(serviceMock.Object, ctx);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            return controller;
        }

        private static ClaimsPrincipal OwnerPrincipal(Guid userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Owner")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task GetRentalsCsv_WhenFromGreaterThanTo_ReturnsBadRequest()
        {               
            using var ctx = CreateInMemoryContext(nameof(GetRentalsCsv_WhenFromGreaterThanTo_ReturnsBadRequest));
            var controller = CreateController(ctx, OwnerPrincipal(Guid.NewGuid()));

            var from = new DateOnly(2026, 2, 10);
            var to = new DateOnly(2026, 2, 1);

            var result = await controller.GetRentalsCsv(from, to);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var bad = (BadRequestObjectResult)result;
            Assert.That(bad.Value?.ToString(), Does.Contain("'from' não pode ser maior que 'to'"));
        }

        [Test]
        public async Task GetRentalsCsv_WithRentals_ReturnsCsvFile_WithHeaderAndRows()
        {
            using var ctx = CreateInMemoryContext(nameof(GetRentalsCsv_WithRentals_ReturnsCsvFile_WithHeaderAndRows));

            // Arrange seed
            var ownerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clothingId = Guid.NewGuid();
            var rentalId = Guid.NewGuid();

            
            var user = new TbUser
            {
                Id = userId,
                Name = "Maria",
                Email = "maria@email.com",
                Telephone = "99999-9999",
                ProfileType = "User",
                Reported = false,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "x"
            };

            var clothing = new TbClothing
            {
                Id = clothingId,
                Description = "Vestido Azul",
                RentPrice = 100,
                AvailabilityStatus = "AVAILABLE",
                OwnerId = ownerId
            };

            var rental = new TbRental
            {
                Id = rentalId,
                UserId = userId,
                ClothingId = clothingId,
                StartDate = new DateOnly(2026, 2, 5),
                EndDate = new DateOnly(2026, 2, 7),
                Status = "COMPLETED",
                TotalValue = 200,

                // Se seu scaffold tem navigation props, mantenha:
                User = user,
                Clothing = clothing
            };

            ctx.TbUsers.Add(user);
            ctx.TbClothings.Add(clothing);
            ctx.TbRentals.Add(rental);
            await ctx.SaveChangesAsync();

            var controller = CreateController(ctx, OwnerPrincipal(ownerId));

            var from = new DateOnly(2026, 2, 1);
            var to = new DateOnly(2026, 2, 28);

            // Act
            var result = await controller.GetRentalsCsv(from, to);

            // Assert File(...)
            Assert.That(result, Is.TypeOf<FileContentResult>());
            var file = (FileContentResult)result;

            Assert.That(file.ContentType, Is.EqualTo("text/csv"));
            Assert.That(file.FileDownloadName, Is.EqualTo("rentals_20260201_20260228.csv"));

            // CSV text (com BOM, porque você usou UTF8Encoding(true))
            var csvText = Encoding.UTF8.GetString(file.FileContents);

            Assert.That(csvText, Does.Contain("RentalId,UserId,UserName,ClothingId,Description,StartDate,EndDate,Status,TotalValue"));
            Assert.That(csvText, Does.Contain(rentalId.ToString()));
            Assert.That(csvText, Does.Contain(userId.ToString()));
            Assert.That(csvText, Does.Contain("Maria"));
            Assert.That(csvText, Does.Contain(clothingId.ToString()));
            Assert.That(csvText, Does.Contain("Vestido Azul"));
            Assert.That(csvText, Does.Contain("2026-02-05"));
            Assert.That(csvText, Does.Contain("2026-02-07"));
            Assert.That(csvText, Does.Contain("COMPLETED"));
            Assert.That(csvText, Does.Contain("200"));
        }

        [Test]
        public void GetRentalsCsv_ShouldHaveAuthorizeRoleOwner()
        {
            var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GetRentalsCsv));
            Assert.That(method, Is.Not.Null);

            var attr = (AuthorizeAttribute?)Attribute.GetCustomAttribute(method!, typeof(AuthorizeAttribute));
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr!.Roles, Is.EqualTo("Owner"));
        }
    }
}
