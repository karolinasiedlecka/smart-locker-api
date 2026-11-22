using Microsoft.EntityFrameworkCore;
using SmartLocker.Application.Interfaces;
using SmartLocker.Domain.Entities;

namespace SmartLocker.Infrastructure.Persistence;

public class SmartLockerStore(AppDbContext dbContext) : ISmartLockerStore
{
    public Task<Parcel?> GetParcelByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
        => dbContext.Parcels
            .Include(p => p.Events)
            .Include(p => p.AssignedCompartment)
                .ThenInclude(c => c!.LockerMachine)
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber, ct);

    public Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default)
        => dbContext.Parcels.AnyAsync(p => p.TrackingNumber == trackingNumber, ct);

    public async Task AddParcelAsync(Parcel parcel, ParcelEvent initialEvent, CancellationToken ct = default)
    {
        await dbContext.Parcels.AddAsync(parcel, ct);
        dbContext.Entry(parcel).State = EntityState.Added;

        await dbContext.ParcelEvents.AddAsync(initialEvent, ct);
        dbContext.Entry(initialEvent).State = EntityState.Added;
    }

    public async Task AddParcelEventAsync(ParcelEvent parcelEvent, CancellationToken ct = default)
    {
        await dbContext.ParcelEvents.AddAsync(parcelEvent, ct);
        dbContext.Entry(parcelEvent).State = EntityState.Added;
    }

    public Task<LockerMachine?> GetLockerWithCompartmentsByCodeAsync(string lockerCode, CancellationToken ct = default)
        => dbContext.LockerMachines
            .Include(l => l.Compartments)
            .FirstOrDefaultAsync(l => l.Code == lockerCode, ct);

    public async Task<IReadOnlyList<LockerMachine>> GetAllLockersWithCompartmentsAsync(CancellationToken ct = default)
        => await dbContext.LockerMachines
            .Include(l => l.Compartments)
            .OrderBy(l => l.Code)
            .ToListAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            await action();
            await transaction.CommitAsync(ct);
        });
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => dbContext.SaveChangesAsync(ct);
}