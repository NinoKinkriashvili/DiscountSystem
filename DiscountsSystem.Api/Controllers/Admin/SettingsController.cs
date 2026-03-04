using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Settings;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Administrator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly ISettingsService _service;

    public SettingsController(ISettingsService service)
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<SettingsDto>> Get(CancellationToken ct)
        => Ok(await _service.GetAsync(ct));

    [HttpPut]
    public async Task<ActionResult<SettingsDto>> Update([FromBody] UpdateSettingsRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAsync(request, ct));
}
