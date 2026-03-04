using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register/customer")]
    public async Task<ActionResult<AuthResponse>> RegisterCustomer(
        [FromBody] RegisterCustomerRequest request,
        CancellationToken ct)
    {
        var res = await _auth.RegisterCustomerAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, res);
    }

    [HttpPost("register/merchant")]
    public async Task<ActionResult<AuthResponse>> RegisterMerchant(
        [FromBody] RegisterMerchantRequest request,
        CancellationToken ct)
    {
        var res = await _auth.RegisterMerchantAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, res);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var res = await _auth.LoginAsync(request, ct);
        return Ok(res);
    }
}
