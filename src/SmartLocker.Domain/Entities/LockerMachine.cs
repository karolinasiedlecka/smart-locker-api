using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLocker.Domain.Entities;

public class LockerMachine
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<LockerCompartment> Compartments { get; set; } = new List<LockerCompartment>();
}