using NUnit.Framework;

namespace Veste_e_Volta.Tests;

[TestFixture]
public class SampleTests
{
    [SetUp]
    public void Setup()
    {
        // Setup executado antes de cada teste
    }

    [Test]
    [Description("Exemplo de teste básico")]
    public void Example_WhenCalled_ShouldPass()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
