using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers;

[ApiController]
[Route("api/test")]
public sealed class TestAuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult Public() => Ok("anyone");

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
        => Ok(new
        {
            name = User.Identity?.Name,
            isAuth = User.Identity?.IsAuthenticated,
            roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToArray()
        });

    [Authorize(Roles = "Administrator")]
    [HttpGet("admin")]
    public IActionResult AdminOnly() => Ok("admin ok");

    [Authorize(Roles = "Customer")]
    [HttpGet("customer")]
    public IActionResult CustomerOnly() => Ok("customer ok");

    [Authorize(Roles = "Merchant")]
    [HttpGet("merchant")]
    public IActionResult MerchantOnly() => Ok("merchant ok");
}