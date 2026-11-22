using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Domain.Enums;

namespace SmartLocker.Application.DTOs;

public record CompartmentResponse(
    Guid Id,
    int CompartmentNumber,
    CompartmentSize Size,
    CompartmentStatus Status);

public record LockerSummaryResponse(
    Guid Id,
    string Code,
    string City,
    string Address,
    bool IsActive,
    IReadOnlyList<CompartmentResponse> Compartments);