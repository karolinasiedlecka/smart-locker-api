using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SmartLocker.Domain.Entities;
using SmartLocker.Domain.Enums;

namespace SmartLocker.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LockerMachine> LockerMachines => Set<LockerMachine>();
    public DbSet<LockerCompartment> LockerCompartments => Set<LockerCompartment>();
    public DbSet<Parcel> Parcels => Set<Parcel>();
    public DbSet<ParcelEvent> ParcelEvents => Set<ParcelEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LockerMachine>(entity =>
        {
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<LockerCompartment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.LockerMachineId, x.CompartmentNumber }).IsUnique();

            entity.HasOne(x => x.LockerMachine)
                .WithMany(x => x.Compartments)
                .HasForeignKey(x => x.LockerMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Parcel>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TrackingNumber).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => x.TrackingNumber).IsUnique();
            entity.Property(x => x.RecipientPhone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PickupCode).HasMaxLength(10);

            entity.HasOne(x => x.AssignedCompartment)
                .WithMany(x => x.Parcels)
                .HasForeignKey(x => x.AssignedCompartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ParcelEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Description).HasMaxLength(300).IsRequired();

            entity.HasOne(x => x.Parcel)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.ParcelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        var wawId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<LockerMachine>().HasData(new LockerMachine
        {
            Id = wawId,
            Code = "WAW-01",
            City = "Warszawa",
            Address = "ul. Marszalkowska 10",
            IsActive = true
        });

        modelBuilder.Entity<LockerCompartment>().HasData(
            new LockerCompartment { Id = Guid.Parse("22222222-2222-2222-2222-222222222201"), LockerMachineId = wawId, CompartmentNumber = 1, Size = CompartmentSize.Small, Status = CompartmentStatus.Available },
            new LockerCompartment { Id = Guid.Parse("22222222-2222-2222-2222-222222222202"), LockerMachineId = wawId, CompartmentNumber = 2, Size = CompartmentSize.Small, Status = CompartmentStatus.Available },
            new LockerCompartment { Id = Guid.Parse("22222222-2222-2222-2222-222222222203"), LockerMachineId = wawId, CompartmentNumber = 3, Size = CompartmentSize.Medium, Status = CompartmentStatus.Available },
            new LockerCompartment { Id = Guid.Parse("22222222-2222-2222-2222-222222222204"), LockerMachineId = wawId, CompartmentNumber = 4, Size = CompartmentSize.Large, Status = CompartmentStatus.Available }
        );
    }
}
