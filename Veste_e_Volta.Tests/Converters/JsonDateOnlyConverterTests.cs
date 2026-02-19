using System.Text;
using System.Text.Json;
using VesteEVolta.Converters;

namespace Veste_e_Volta.Tests.Converters;

[TestFixture]
public class JsonDateOnlyConverterTests
{
    private JsonDateOnlyConverter _converter;
    private JsonSerializerOptions _options;

    [SetUp]
    public void Setup()
    {
        _converter = new JsonDateOnlyConverter();
        _options = new JsonSerializerOptions
        {
            Converters = { _converter }
        };
    }

    #region Read Tests

    [Test]
    public void Read_ValidDateString_ReturnsDateOnly()
    {
        // Arrange
        var json = "\"2024-12-25\"";
        var expectedDate = new DateOnly(2024, 12, 25);

        // Act
        var result = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDate));
    }

    [Test]
    public void Read_ValidDateStringYYYYMMDD_ReturnsDateOnly()
    {
        // Arrange
        var json = "\"2026-02-19\"";
        var expectedDate = new DateOnly(2026, 2, 19);

        // Act
        var result = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDate));
    }

    [Test]
    public void Read_LeapYearDate_ReturnsDateOnly()
    {
        // Arrange
        var json = "\"2024-02-29\"";
        var expectedDate = new DateOnly(2024, 2, 29);

        // Act
        var result = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDate));
    }

    [Test]
    public void Read_FirstDayOfYear_ReturnsDateOnly()
    {
        // Arrange
        var json = "\"2025-01-01\"";
        var expectedDate = new DateOnly(2025, 1, 1);

        // Act
        var result = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDate));
    }

    [Test]
    public void Read_LastDayOfYear_ReturnsDateOnly()
    {
        // Arrange
        var json = "\"2025-12-31\"";
        var expectedDate = new DateOnly(2025, 12, 31);

        // Act
        var result = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDate));
    }

    [Test]
    public void Read_InvalidDateFormat_ThrowsJsonException()
    {
        // Arrange
        var json = "\"2024-13-45\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    [Test]
    public void Read_InvalidDateSlashFormat_ThrowsJsonException()
    {
        // Arrange
        var json = "\"invalid-date-text\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    [Test]
    public void Read_EmptyString_ThrowsJsonException()
    {
        // Arrange
        var json = "\"\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    [Test]
    public void Read_InvalidMonth_ThrowsJsonException()
    {
        // Arrange
        var json = "\"2024-13-01\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    [Test]
    public void Read_InvalidDay_ThrowsJsonException()
    {
        // Arrange
        var json = "\"2024-02-30\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    [Test]
    public void Read_NullValue_ThrowsJsonException()
    {
        // Arrange
        var json = "null";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));
    }

    [Test]
    public void Read_NumberInsteadOfString_ThrowsJsonException()
    {
        // Arrange
        var json = "20241225";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));
    }

    [Test]
    public void Read_TextInsteadOfDate_ThrowsJsonException()
    {
        // Arrange
        var json = "\"not a date\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<DateOnly>(json, _options));

        Assert.That(ex!.Message, Does.Contain("Invalid date format"));
    }

    #endregion

    #region Write Tests

    [Test]
    public void Write_ValidDateOnly_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2024, 12, 25);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2024-12-25\""));
    }

    [Test]
    public void Write_DateOnlyWithSingleDigitMonth_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 15);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2024-01-15\""));
    }

    [Test]
    public void Write_DateOnlyWithSingleDigitDay_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2024, 12, 5);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2024-12-05\""));
    }

    [Test]
    public void Write_DateOnlyFirstDayOfYear_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2025, 1, 1);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2025-01-01\""));
    }

    [Test]
    public void Write_DateOnlyLastDayOfYear_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2025, 12, 31);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2025-12-31\""));
    }

    [Test]
    public void Write_DateOnlyLeapYear_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 29);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"2024-02-29\""));
    }

    [Test]
    public void Write_MinDate_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(1, 1, 1);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"0001-01-01\""));
    }

    [Test]
    public void Write_MaxDate_ReturnsFormattedString()
    {
        // Arrange
        var date = new DateOnly(9999, 12, 31);

        // Act
        var json = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json, Is.EqualTo("\"9999-12-31\""));
    }

    #endregion

    #region Round-trip Tests

    [Test]
    public void RoundTrip_SerializeAndDeserialize_PreservesValue()
    {
        // Arrange
        var originalDate = new DateOnly(2024, 6, 15);

        // Act
        var json = JsonSerializer.Serialize(originalDate, _options);
        var deserializedDate = JsonSerializer.Deserialize<DateOnly>(json, _options);

        // Assert
        Assert.That(deserializedDate, Is.EqualTo(originalDate));
    }

    [Test]
    public void RoundTrip_MultipleSerializations_MaintainConsistency()
    {
        // Arrange
        var date = new DateOnly(2026, 2, 19);

        // Act
        var json1 = JsonSerializer.Serialize(date, _options);
        var json2 = JsonSerializer.Serialize(date, _options);

        // Assert
        Assert.That(json1, Is.EqualTo(json2));
    }

    [Test]
    public void RoundTrip_InComplexObject_WorksCorrectly()
    {
        // Arrange
        var obj = new { EventDate = new DateOnly(2024, 12, 25), Name = "Christmas" };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!["EventDate"].GetString(), Is.EqualTo("2024-12-25"));
        Assert.That(result["Name"].GetString(), Is.EqualTo("Christmas"));
    }

    #endregion
}
