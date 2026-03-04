using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Categories;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Administrator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/categories")]
public sealed class AdminCategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public AdminCategoriesController(ICategoryService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var id = await _service.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    // Update NAME only
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateName(int id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateNameAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }

    // Update STATUS only
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateCategoryStatusRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateStatusAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }

    // optional: DELETE -> deactivate
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeactivateAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
