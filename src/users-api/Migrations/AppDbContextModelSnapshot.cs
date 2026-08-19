using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Prometheus.Users.Api.Data;

#nullable disable

namespace Prometheus.Users.Api.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.14");

        modelBuilder.Entity("Prometheus.Users.Api.Models.User", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            entity.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnType("TEXT");

            entity.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("TEXT");

            entity.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT");

            entity.Property<string>("PasswordHash")
                .IsRequired()
                .HasColumnType("TEXT");

            entity.Property<int>("Role")
                .HasColumnType("INTEGER");

            entity.HasKey("Id");

            entity.HasIndex("Email")
                .IsUnique();

            entity.ToTable("Users");
        });
#pragma warning restore 612, 618
    }
}
