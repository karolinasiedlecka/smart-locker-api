using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Application.DTOs;
using SmartLocker.Application.Exceptions;
using SmartLocker.Application.Interfaces;
using SmartLocker.Domain.Entities;

namespace SmartLocker.Application.Services;

public class LockerService(ISmartLockerStore store) : ILockerService
{
    public async Task<IReadOnlyList<LockerSummaryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var lockers = await store.GetAllLockersWithCompartmentsAsync(ct);
        return lockers.Select(MapToSummary).ToList();
    }

    public async Task<LockerSummaryResponse> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var locker = await store.GetLockerWithCompartmentsByCodeAsync(code.Trim().ToUpperInvariant(), ct)
            ?? throw new NotFoundException($"Nie znaleziono automatu '{code}'.");

        return MapToSummary(locker);
    }

    private static LockerSummaryResponse MapToSummary(LockerMachine locker)
    {
        var compartments = locker.Compartments
            .OrderBy(c => c.CompartmentNumber)
            .Select(c => new CompartmentResponse(c.Id, c.CompartmentNumber, c.Size, c.Status))
            .ToList();

        return new LockerSummaryResponse(
            locker.Id,
            locker.Code,
            locker.City,
            locker.Address,
            locker.IsActive,
            compartments);
    }
}