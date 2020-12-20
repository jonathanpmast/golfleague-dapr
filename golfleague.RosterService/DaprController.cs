using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace golfleague.RosterService
{

    [ApiController]
    [Route("[controller]")]
    public class DaprController : ControllerBase
    {
        private readonly string pubsubName;
        ILogger<DaprController> _logger;
        private readonly IConfiguration _configuration;
        public DaprController(ILogger<DaprController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            pubsubName = _configuration["DaprComponentNames:pubsub.GolfLeague"];
        }

        [HttpGet("subscribe")]
        public ActionResult<IEnumerable<string>> Subscribe()
        {
            return new OkObjectResult(
                new[] { new { pubsubname = this.pubsubName, topic = "scores", route = "/newgolferscore" } }
            );
        }
    }
}
