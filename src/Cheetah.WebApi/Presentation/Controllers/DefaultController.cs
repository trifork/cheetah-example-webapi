using Microsoft.AspNetCore.Mvc;

namespace Cheetah.WebApi.Presentation.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class DefaultController : ControllerBase
{
    [HttpGet("/")]
    public RedirectResult RedirectToSwagger()
    {
        return RedirectPermanent("/swagger");
    }
}
