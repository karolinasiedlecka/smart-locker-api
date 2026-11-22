using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Domain.Enums;

namespace SmartLocker.Application.DTOs;

public record RegisterParcelRequest(
    string RecipientPhone,
    CompartmentSize RequiredSize);

public record DepositParcelRequest(
    string LockerCode);

public record PickupParcelRequest(
    string LockerCode,
    string TrackingNumber,
    string PickupCode);

public record ParcelEventResponse(
    ParcelEventType EventType,
    string Description,
    DateTime OccurredAt);

public record ParcelResponse(
    Guid Id,
    string TrackingNumber,
    string RecipientPhone,
    CompartmentSize RequiredSize,
    ParcelStatus Status,
    string? LockerCode,
    int? CompartmentNumber,
    string? PickupCode,
    DateTime CreatedAt,
    DateTime? DepositedAt,
    DateTime? ExpiresAt,
    DateTime? PickedUpAt,
    IReadOnlyList<ParcelEventResponse> Events);

public record DepositParcelResponse(
    string TrackingNumber,
    string LockerCode,
    int CompartmentNumber,
    string PickupCode,
    DateTime DepositedAt,
    DateTime ExpiresAt,
    ParcelStatus Status);

public record PickupParcelResponse(
    string TrackingNumber,
    string LockerCode,
    int CompartmentNumber,
    DateTime PickedUpAt,
    ParcelStatus Status);