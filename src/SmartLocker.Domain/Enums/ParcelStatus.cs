using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLocker.Domain.Enums;

public enum ParcelStatus
{
    Created = 1,
    InLocker = 2,
    PickedUp = 3,
    Expired = 4,
    Returned = 5
}