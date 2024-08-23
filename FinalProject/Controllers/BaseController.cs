using Microsoft.AspNetCore.Mvc;
namespace finalproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        // private ILogger<T>? logger;
        // protected ILogger<T>? Logger => logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        private readonly ILogger<T> _logger;

        protected ILogger<T> Logger => _logger;

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}