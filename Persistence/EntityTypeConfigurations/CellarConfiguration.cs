using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WineCellar.Domain;

namespace WineCellar.Persistence.EntityTypeConfigurations;

public class CellarConfiguration : IEntityTypeConfiguration<Cellar>
{
    public void Configure(EntityTypeBuilder<Cellar> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Name);
        builder.Property(e => e.Name).HasMaxLength(50);
        builder.Property(e => e.Location).HasMaxLength(100);
        builder
            .HasMany(e => e.Storages)
            .WithOne(e => e.Cellar)
            .HasForeignKey(e => e.CellarId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Users).WithMany(e => e.Cellars);
    }
}
