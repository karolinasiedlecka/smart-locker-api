using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Domain.Enums;

namespace SmartLocker.Domain.Entities;

public class LockerCompartment
{
    public Guid Id { get; set; }
    public Guid LockerMachineId { get; set; }
    public int CompartmentNumber { get; set; }
    public CompartmentSize Size { get; set; }
    public CompartmentStatus Status { get; set; } = CompartmentStatus.Available;

    public LockerMachine LockerMachine { get; set; } = null!;
    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
}