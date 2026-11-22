using System.Security.Cryptography;
using SmartLocker.Application.DTOs;
using SmartLocker.Application.Exceptions;
using SmartLocker.Application.Interfaces;
using SmartLocker.Domain.Entities;
using SmartLocker.Domain.Enums;

namespace SmartLocker.Application.Services;

public class ParcelService(ISmartLockerStore store) : IParcelService
{
    public async Task<ParcelResponse> RegisterAsync(RegisterParcelRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RecipientPhone))
            throw new DomainValidationException("Numer telefonu odbiorcy jest wymagany.");

        if (!Enum.IsDefined(request.RequiredSize))
            throw new DomainValidationException("Niepoprawny rozmiar skrytki.");

        var trackingNumber = await GenerateUniqueTrackingNumberAsync(ct);
        var now = DateTime.UtcNow;

        var parcel = new Parcel
        {
            Id = Guid.NewGuid(),
            TrackingNumber = trackingNumber,
            RecipientPhone = request.RecipientPhone.Trim(),
            RequiredSize = request.RequiredSize,
            Status = ParcelStatus.Created,
            CreatedAt = now
        };

        var parcelEvent = new ParcelEvent
        {
            Id = Guid.NewGuid(),
            ParcelId = parcel.Id,
            EventType = ParcelEventType.Registered,
            Description = $"Zarejestrowano przesyłkę o rozmiarze {parcel.RequiredSize}.",
            OccurredAt = now
        };

        parcel.Events.Add(parcelEvent);
        await store.AddParcelAsync(parcel, parcelEvent, ct);
        await store.SaveChangesAsync(ct);

        return MapToResponse(parcel);
    }

    public async Task<ParcelResponse> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
    {
        var parcel = await store.GetParcelByTrackingNumberAsync(trackingNumber.Trim(), ct)
            ?? throw new NotFoundException($"Nie znaleziono przesyłki o numerze '{trackingNumber}'.");

        return MapToResponse(parcel);
    }

    public async Task<DepositParcelResponse> DepositAsync(
        string trackingNumber,
        DepositParcelRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.LockerCode))
            throw new DomainValidationException("Kod automatu paczkowego jest wymagany.");

        var normalizedTracking = trackingNumber.Trim();
        var normalizedLockerCode = request.LockerCode.Trim().ToUpperInvariant();

        DepositParcelResponse? result = null;

        await store.ExecuteInTransactionAsync(async () =>
        {
            var parcel = await store.GetParcelByTrackingNumberAsync(normalizedTracking, ct)
                ?? throw new NotFoundException($"Nie znaleziono przesyłki '{normalizedTracking}'.");

            if (parcel.Status != ParcelStatus.Created)
                throw new ConflictException($"Przesyłka w statusie '{parcel.Status}' nie może zostać umieszczona w skrytce.");

            var locker = await store.GetLockerWithCompartmentsByCodeAsync(normalizedLockerCode, ct)
                ?? throw new NotFoundException($"Nie znaleziono automatu '{normalizedLockerCode}'.");

            if (!locker.IsActive)
                throw new ConflictException($"Automat '{locker.Code}' jest obecnie nieaktywny.");

            var compartment = locker.Compartments
                .Where(c => c.Status == CompartmentStatus.Available && c.Size >= parcel.RequiredSize)
                .OrderBy(c => c.Size)
                .ThenBy(c => c.CompartmentNumber)
                .FirstOrDefault()
                ?? throw new ConflictException(
                    $"Brak wolnej skrytki o rozmiarze co najmniej '{parcel.RequiredSize}' w automacie '{locker.Code}'.");

            var now = DateTime.UtcNow;
            var pickupCode = GeneratePickupCode();

            compartment.Status = CompartmentStatus.Occupied;

            parcel.AssignedCompartmentId = compartment.Id;
            parcel.AssignedCompartment = compartment;
            parcel.PickupCode = pickupCode;
            parcel.Status = ParcelStatus.InLocker;
            parcel.DepositedAt = now;
            parcel.ExpiresAt = now.AddHours(48);

            var depositEvent = new ParcelEvent
            {
                Id = Guid.NewGuid(),
                ParcelId = parcel.Id,
                EventType = ParcelEventType.Deposited,
                Description = $"Umieszczono w automacie {locker.Code}, skrytka nr {compartment.CompartmentNumber}.",
                OccurredAt = now
            };

            parcel.Events.Add(depositEvent);
            await store.AddParcelEventAsync(depositEvent, ct);
            await store.SaveChangesAsync(ct);

            result = new DepositParcelResponse(
                parcel.TrackingNumber,
                locker.Code,
                compartment.CompartmentNumber,
                pickupCode,
                now,
                parcel.ExpiresAt.Value,
                parcel.Status);
        }, ct);

        return result!;
    }

    public async Task<PickupParcelResponse> PickupAsync(PickupParcelRequest request, CancellationToken ct = default)
    {
        var normalizedTracking = request.TrackingNumber.Trim();
        var normalizedLockerCode = request.LockerCode.Trim().ToUpperInvariant();
        var normalizedPickupCode = request.PickupCode.Trim();

        PickupParcelResponse? result = null;

        await store.ExecuteInTransactionAsync(async () =>
        {
            var parcel = await store.GetParcelByTrackingNumberAsync(normalizedTracking, ct)
                ?? throw new NotFoundException($"Nie znaleziono przesyłki '{normalizedTracking}'.");

            if (parcel.Status != ParcelStatus.InLocker)
                throw new ConflictException($"Przesyłka w statusie '{parcel.Status}' nie jest gotowa do odbioru.");

            if (parcel.AssignedCompartment is null)
                throw new ConflictException("Przesyłka nie ma przypisanej skrytki.");

            var compartment = parcel.AssignedCompartment;
            var lockerCode = compartment.LockerMachine?.Code ?? string.Empty;

            if (!string.Equals(lockerCode, normalizedLockerCode, StringComparison.OrdinalIgnoreCase))
                throw new DomainValidationException($"Przesyłka nie znajduje się w automacie '{normalizedLockerCode}'.");

            if (!string.Equals(parcel.PickupCode, normalizedPickupCode, StringComparison.Ordinal))
                throw new DomainValidationException("Niepoprawny kod odbioru.");

            var now = DateTime.UtcNow;
            if (parcel.ExpiresAt.HasValue && parcel.ExpiresAt.Value < now)
            {
                parcel.Status = ParcelStatus.Expired;
                compartment.Status = CompartmentStatus.Available;
                parcel.AssignedCompartmentId = null;

                var expiredEvent = new ParcelEvent
                {
                    Id = Guid.NewGuid(),
                    ParcelId = parcel.Id,
                    EventType = ParcelEventType.Expired,
                    Description = $"Przekroczono termin odbioru w skrytce nr {compartment.CompartmentNumber} automatu {lockerCode}.",
                    OccurredAt = now
                };

                parcel.Events.Add(expiredEvent);
                await store.AddParcelEventAsync(expiredEvent, ct);
                await store.SaveChangesAsync(ct);

                throw new ConflictException("Termin odbioru przesyłki upłynął.");
            }

            var compartmentNumber = compartment.CompartmentNumber;

            parcel.Status = ParcelStatus.PickedUp;
            parcel.PickedUpAt = now;
            parcel.AssignedCompartmentId = null;

            compartment.Status = CompartmentStatus.Available;

            var pickupEvent = new ParcelEvent
            {
                Id = Guid.NewGuid(),
                ParcelId = parcel.Id,
                EventType = ParcelEventType.PickedUp,
                Description = $"Odebrano przesyłkę ze skrytki nr {compartmentNumber} automatu {lockerCode}.",
                OccurredAt = now
            };

            parcel.Events.Add(pickupEvent);
            await store.AddParcelEventAsync(pickupEvent, ct);
            await store.SaveChangesAsync(ct);

            result = new PickupParcelResponse(
                parcel.TrackingNumber,
                lockerCode,
                compartmentNumber,
                now,
                parcel.Status);
        }, ct);

        return result!;
    }

    private async Task<string> GenerateUniqueTrackingNumberAsync(CancellationToken ct)
    {
        for (var i = 0; i < 10; i++)
        {
            var token = RandomNumberGenerator.GetInt32(100_000_000, 999_999_999);
            var candidate = $"SLK-{token}";
            if (!await store.TrackingNumberExistsAsync(candidate, ct))
                return candidate;
        }

        return $"SLK-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}";
    }

    private static string GeneratePickupCode()
        => RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

    private static ParcelResponse MapToResponse(Parcel parcel)
    {
        var events = parcel.Events
            .OrderBy(e => e.OccurredAt)
            .Select(e => new ParcelEventResponse(e.EventType, e.Description, e.OccurredAt))
            .ToList();

        return new ParcelResponse(
            parcel.Id,
            parcel.TrackingNumber,
            parcel.RecipientPhone,
            parcel.RequiredSize,
            parcel.Status,
            parcel.AssignedCompartment?.LockerMachine?.Code,
            parcel.AssignedCompartment?.CompartmentNumber,
            parcel.PickupCode,
            parcel.CreatedAt,
            parcel.DepositedAt,
            parcel.ExpiresAt,
            parcel.PickedUpAt,
            events);
    }
}