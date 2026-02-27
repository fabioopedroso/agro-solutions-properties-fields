using Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Tests.Core.ValueObjects;

public class AddressTests
{
    [Fact]
    public void CreateAddress_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234-567";
        var country = "Brasil";

        // Act
        var address = new Address(street, number, city, state, zipCode, country);

        // Assert
        Assert.Equal(street, address.Street);
        Assert.Equal(number, address.Number);
        Assert.Equal(city, address.City);
        Assert.Equal(state, address.State);
        Assert.Equal(zipCode, address.ZipCode);
        Assert.Equal(country, address.Country);
        Assert.Null(address.Complement);
    }

    [Fact]
    public void CreateAddress_WithComplement_ShouldCreateSuccessfully()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234-567";
        var country = "Brasil";
        var complement = "Apto 101";

        // Act
        var address = new Address(street, number, city, state, zipCode, country, complement);

        // Assert
        Assert.Equal(complement, address.Complement);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidStreet_ShouldThrowValidationException(string invalidStreet)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address(invalidStreet, "123", "São Paulo", "SP", "01234-567", "Brasil"));

        Assert.Equal("A rua é obrigatória.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidNumber_ShouldThrowValidationException(string invalidNumber)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address("Rua das Flores", invalidNumber, "São Paulo", "SP", "01234-567", "Brasil"));

        Assert.Equal("O número é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidCity_ShouldThrowValidationException(string invalidCity)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address("Rua das Flores", "123", invalidCity, "SP", "01234-567", "Brasil"));

        Assert.Equal("A cidade é obrigatória.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidState_ShouldThrowValidationException(string invalidState)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address("Rua das Flores", "123", "São Paulo", invalidState, "01234-567", "Brasil"));

        Assert.Equal("O estado é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidZipCode_ShouldThrowValidationException(string invalidZipCode)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address("Rua das Flores", "123", "São Paulo", "SP", invalidZipCode, "Brasil"));

        Assert.Equal("O CEP é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAddress_WithInvalidCountry_ShouldThrowValidationException(string invalidCountry)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", invalidCountry));

        Assert.Equal("O país é obrigatório.", exception.Message);
    }

    [Fact]
    public void GetFullAddress_WithoutComplement_ShouldFormatCorrectly()
    {
        // Arrange
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        Assert.Equal("Rua das Flores, 123 - São Paulo/SP, 01234-567 - Brasil", fullAddress);
    }

    [Fact]
    public void GetFullAddress_WithComplement_ShouldFormatCorrectly()
    {
        // Arrange
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        Assert.Equal("Rua das Flores, 123, Apto 101 - São Paulo/SP, 01234-567 - Brasil", fullAddress);
    }

    [Fact]
    public void ToString_ShouldReturnFullAddress()
    {
        // Arrange
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");

        // Act
        var result = address.ToString();

        // Assert
        Assert.Equal(address.GetFullAddress(), result);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");
        var address2 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");

        // Act
        var result = address1.Equals(address2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithDifferentStreet_ShouldReturnFalse()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var address2 = new Address("Rua das Árvores", "123", "São Paulo", "SP", "01234-567", "Brasil");

        // Act
        var result = address1.Equals(address2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithDifferentNumber_ShouldReturnFalse()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var address2 = new Address("Rua das Flores", "456", "São Paulo", "SP", "01234-567", "Brasil");

        // Act
        var result = address1.Equals(address2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithDifferentComplement_ShouldReturnFalse()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");
        var address2 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 102");

        // Act
        var result = address1.Equals(address2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");

        // Act
        var result = address.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var address = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var otherObject = "Not an address";

        // Act
        var result = address.Equals(otherObject);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");
        var address2 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil", "Apto 101");

        // Act
        var hash1 = address1.GetHashCode();
        var hash2 = address2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var address1 = new Address("Rua das Flores", "123", "São Paulo", "SP", "01234-567", "Brasil");
        var address2 = new Address("Rua das Árvores", "456", "Rio de Janeiro", "RJ", "20000-000", "Brasil");

        // Act
        var hash1 = address1.GetHashCode();
        var hash2 = address2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
