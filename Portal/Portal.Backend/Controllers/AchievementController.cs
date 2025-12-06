using Microsoft.AspNetCore.Mvc;
using Portal.Database.Models;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AchievementController : ControllerBase
    {
        private readonly ILogger<AchievementController> _logger;

        public AchievementController(ILogger<AchievementController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public UserAchievements GetUserAchievements()
        {
            return new();
        }
    }
}
