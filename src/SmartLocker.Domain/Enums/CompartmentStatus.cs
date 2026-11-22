using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLocker.Domain.Enums;

public enum CompartmentStatus
{
    Available = 1,
    Occupied = 2,
    OutOfOrder = 3
}