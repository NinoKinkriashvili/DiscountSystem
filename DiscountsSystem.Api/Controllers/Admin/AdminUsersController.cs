using Asp.Versioning;
using DiscountsSystem.Application.DTOs.AdminUsers;
using DiscountsSystem.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Administrator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IAuthService _auth;

    public AdminUsersController(IAuthService auth)
    {
        _auth = auth;
    }

    // Admin creates a Customer (NO TOKEN)
    [HttpPost("customers")]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerByAdminRequest request,
        CancellationToken ct)
    {
        var created = await _auth.CreateCustomerByAdminAsync(request, ct);

        // 201 + payload
        return StatusCode(StatusCodes.Status201Created, created);
    }

    // Admin creates a Merchant (NO TOKEN)
    [HttpPost("merchants")]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMerchant(
        [FromBody] CreateMerchantByAdminRequest request,
        CancellationToken ct)
    {
        var created = await _auth.CreateMerchantByAdminAsync(request, ct);

        return StatusCode(StatusCodes.Status201Created, created);
    }
}
