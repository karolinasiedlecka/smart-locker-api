using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Domain.Enums;

namespace SmartLocker.Domain.Entities;

public class Parcel
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public CompartmentSize RequiredSize { get; set; }
    public ParcelStatus Status { get; set; } = ParcelStatus.Created;

    public Guid? AssignedCompartmentId { get; set; }
    public string? PickupCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DepositedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? PickedUpAt { get; set; }

    public LockerCompartment? AssignedCompartment { get; set; }
    public ICollection<ParcelEvent> Events { get; set; } = new List<ParcelEvent>();
}