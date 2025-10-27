using Microsoft.AspNetCore.Mvc;
using Portal.Database.Models;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public User Get()
        {
            return new();
        }
    }
}
