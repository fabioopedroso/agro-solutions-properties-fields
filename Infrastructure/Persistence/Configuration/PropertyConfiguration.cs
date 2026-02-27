using Core.Entities;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Property");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnType("INT")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnType("VARCHAR(200)")
            .IsRequired();

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasColumnType("VARCHAR(200)")
                .IsRequired();

            address.Property(a => a.Number)
                .HasColumnName("AddressNumber")
                .HasColumnType("VARCHAR(20)")
                .IsRequired();

            address.Property(a => a.Complement)
                .HasColumnName("AddressComplement")
                .HasColumnType("VARCHAR(100)");

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasColumnType("VARCHAR(100)")
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("AddressState")
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

            address.Property(a => a.ZipCode)
                .HasColumnName("AddressZipCode")
                .HasColumnType("VARCHAR(20)")
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasColumnType("VARCHAR(50)")
                .IsRequired();
        });

        builder.OwnsOne(p => p.Coordinates, coordinates =>
        {
            coordinates.Property(c => c.Latitude)
                .HasColumnName("Latitude")
                .HasColumnType("DOUBLE PRECISION")
                .IsRequired();

            coordinates.Property(c => c.Longitude)
                .HasColumnName("Longitude")
                .HasColumnType("DOUBLE PRECISION")
                .IsRequired();
        });

        builder.Property(p => p.TotalArea)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        builder.Property(p => p.SoilType)
            .HasColumnType("VARCHAR(100)")
            .IsRequired();

        builder.Property(p => p.UserId)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .IsRequired();

        builder.HasMany(p => p.Fields)
            .WithOne(f => f.Property)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.UserId);
    }
}
