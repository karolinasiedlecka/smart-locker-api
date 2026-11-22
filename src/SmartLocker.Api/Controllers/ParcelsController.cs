using Microsoft.AspNetCore.Mvc;
using SmartLocker.Application.DTOs;
using SmartLocker.Application.Interfaces;

namespace SmartLocker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParcelsController(IParcelService parcelService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ParcelResponse>> Register(
        [FromBody] RegisterParcelRequest request,
        CancellationToken ct)
    {
        var response = await parcelService.RegisterAsync(request, ct);
        return CreatedAtAction(nameof(GetByTrackingNumber), new { trackingNumber = response.TrackingNumber }, response);
    }

    [HttpGet("{trackingNumber}")]
    public async Task<ActionResult<ParcelResponse>> GetByTrackingNumber(
        string trackingNumber,
        CancellationToken ct)
    {
        var response = await parcelService.GetByTrackingNumberAsync(trackingNumber, ct);
        return Ok(response);
    }

    [HttpPost("{trackingNumber}/deposit")]
    public async Task<ActionResult<DepositParcelResponse>> Deposit(
        string trackingNumber,
        [FromBody] DepositParcelRequest request,
        CancellationToken ct)
    {
        var response = await parcelService.DepositAsync(trackingNumber, request, ct);
        return Ok(response);
    }

    [HttpPost("pickup")]
    public async Task<ActionResult<PickupParcelResponse>> Pickup(
        [FromBody] PickupParcelRequest request,
        CancellationToken ct)
    {
        var response = await parcelService.PickupAsync(request, ct);
        return Ok(response);
    }
}