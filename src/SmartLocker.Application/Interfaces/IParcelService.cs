using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartLocker.Application.DTOs;

namespace SmartLocker.Application.Interfaces;

public interface IParcelService
{
    Task<ParcelResponse> RegisterAsync(RegisterParcelRequest request, CancellationToken ct = default);
    Task<ParcelResponse> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<DepositParcelResponse> DepositAsync(string trackingNumber, DepositParcelRequest request, CancellationToken ct = default);
    Task<PickupParcelResponse> PickupAsync(PickupParcelRequest request, CancellationToken ct = default);
}