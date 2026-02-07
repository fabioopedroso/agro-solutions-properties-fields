using System.ComponentModel.DataAnnotations;

namespace Core.ValueObjects;

public class Address
{
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string? Complement { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string number, string city, string state, string zipCode, string country, string? complement = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ValidationException("A rua é obrigatória.");

        if (string.IsNullOrWhiteSpace(number))
            throw new ValidationException("O número é obrigatório.");

        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationException("A cidade é obrigatória.");

        if (string.IsNullOrWhiteSpace(state))
            throw new ValidationException("O estado é obrigatório.");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ValidationException("O CEP é obrigatório.");

        if (string.IsNullOrWhiteSpace(country))
            throw new ValidationException("O país é obrigatório.");

        Street = street;
        Number = number;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        Complement = complement;
    }

    private Address()
    {
        Street = string.Empty;
        Number = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
    }

    public string GetFullAddress()
    {
        var address = $"{Street}, {Number}";
        if (!string.IsNullOrWhiteSpace(Complement))
            address += $", {Complement}";
        address += $" - {City}/{State}, {ZipCode} - {Country}";
        return address;
    }

    public override string ToString() => GetFullAddress();

    public override bool Equals(object? obj)
    {
        if (obj is Address other)
        {
            return Street == other.Street &&
                   Number == other.Number &&
                   City == other.City &&
                   State == other.State &&
                   ZipCode == other.ZipCode &&
                   Country == other.Country &&
                   Complement == other.Complement;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Number, City, State, ZipCode, Country, Complement);
    }
}
