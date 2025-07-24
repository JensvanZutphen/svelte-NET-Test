using Microsoft.AspNetCore.Mvc;
using MySvelteApp.Server.Models;

namespace MySvelteApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TestAuthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "If you can see this, you are authenticated!" });
    }
} 