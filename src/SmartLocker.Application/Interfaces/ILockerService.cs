using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Application.DTOs;

namespace SmartLocker.Application.Interfaces;

public interface ILockerService
{
    Task<IReadOnlyList<LockerSummaryResponse>> GetAllAsync(CancellationToken ct = default);
    Task<LockerSummaryResponse> GetByCodeAsync(string code, CancellationToken ct = default);
}