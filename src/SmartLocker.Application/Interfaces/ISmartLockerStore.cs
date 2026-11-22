using SmartLocker.Domain.Entities;

namespace SmartLocker.Application.Interfaces;

public interface ISmartLockerStore
{
    Task<Parcel?> GetParcelByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);
    Task AddParcelAsync(Parcel parcel, ParcelEvent initialEvent, CancellationToken ct = default);
    Task AddParcelEventAsync(ParcelEvent parcelEvent, CancellationToken ct = default);

    Task<LockerMachine?> GetLockerWithCompartmentsByCodeAsync(string lockerCode, CancellationToken ct = default);
    Task<IReadOnlyList<LockerMachine>> GetAllLockersWithCompartmentsAsync(CancellationToken ct = default);

    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}