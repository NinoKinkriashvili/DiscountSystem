using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Categories;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Public;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/public/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<ActionResult<List<CategoryListItemDto>>> GetActive(CancellationToken ct)
    {
        var result = await _categoryService.GetActiveAsync(ct);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {
        var result = await _categoryService.GetActiveByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
