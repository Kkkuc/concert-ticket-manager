using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController(IHomeService homeService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetMessage()
    {
        var text = homeService.GetWelcomeMessage();
        return Ok(new { message = text });
    }
}