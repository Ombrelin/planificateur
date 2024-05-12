using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Planificateur.Web.Controllers;

[AllowAnonymous]
[Route("")]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomePageController : Controller
{
    [HttpGet]
    public IActionResult HomePage() => View("Views/Home/Home.cshtml");
}