using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Domain.Enums;

namespace SmartLocker.Domain.Entities;

public class ParcelEvent
{
    public Guid Id { get; set; }
    public Guid ParcelId { get; set; }
    public ParcelEventType EventType { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public Parcel Parcel { get; set; } = null!;
}