using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace golfleague.RosterService
{
    [ApiController]
    [Route("[controller")]
    public class RosterController : ControllerBase
    {
        private readonly string daprPort, stateStoreName, pubsubName;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RosterController> _logger;

        private readonly IConfiguration _configuration;
        public RosterController(IHttpClientFactory httpClientFactory,
                                ILogger<RosterController> logger,
                                IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            daprPort = _configuration["DAPR_HTTP_PORT"];
            stateStoreName = _configuration["DaprComponentNames:statestore.local"];
            pubsubName = _configuration["DaprComponentNames:pubsub.GolfLeague"];
        }

        [HttpPost("/{leagueid}/[controller]/golfers")]
        public async Task<IActionResult> NewGolfer(CloudEvent cloudEvent)
        {
            var golfer = ((JToken)cloudEvent.Data).ToObject<Golfer>();
            _logger.LogInformation("Received Golfer: {@Golfer}", golfer);
            await Task.Delay(500);
            return Ok();
        }

        [HttpPost("/newgolferscore")]
        public async Task<IActionResult> GolferScoreAdded(CloudEvent cloudEvent)
        {
            var jtoken = cloudEvent.Data as JToken;
            var score = ((JToken)cloudEvent.Data).ToObject<Score>();
            await Task.Delay(500);
            return Ok();
        }
    }
}