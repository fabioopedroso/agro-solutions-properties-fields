using Core.Entities;
using Core.Enums;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("Field");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasColumnType("INT")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Name)
            .HasColumnType("VARCHAR(200)")
            .IsRequired();

        builder.Property(f => f.CropType)
            .HasColumnType("VARCHAR(100)")
            .IsRequired();

        builder.Property(f => f.Area)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        builder.Property(f => f.PlantingDate)
            .HasColumnType("TIMESTAMPTZ");

        builder.OwnsOne(f => f.Coordinates, coordinates =>
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

        builder.Property(f => f.Status)
            .HasColumnType("INT")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(f => f.PropertyId)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .HasColumnType("TIMESTAMPTZ")
            .IsRequired();

        builder.HasOne(f => f.Property)
            .WithMany(p => p.Fields)
            .HasForeignKey(f => f.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.PropertyId);
    }
}
