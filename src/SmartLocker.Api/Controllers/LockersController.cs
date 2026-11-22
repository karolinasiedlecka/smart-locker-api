using Microsoft.AspNetCore.Mvc;
using SmartLocker.Application.DTOs;
using SmartLocker.Application.Interfaces;

namespace SmartLocker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LockersController(ILockerService lockerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LockerSummaryResponse>>> GetAll(CancellationToken ct)
    {
        var response = await lockerService.GetAllAsync(ct);
        return Ok(response);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<LockerSummaryResponse>> GetByCode(string code, CancellationToken ct)
    {
        var response = await lockerService.GetByCodeAsync(code, ct);
        return Ok(response);
    }
}