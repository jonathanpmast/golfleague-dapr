using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace golfleague.ScoreService
{
    [ApiController]
    [Route("[controller]")]
    public class ScoresController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<ScoresController> _logger;
        public ScoresController(IHttpClientFactory httpClientFactory,
                                ILogger<ScoresController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Score>> GetScores()
        {
            await Task.Delay(500);
            return new List<Score>();
        }

        public async Task<IEnumerable<Score>> GetScoresByGolfer()
        {
            await Task.Delay(500);
            return new List<Score>();
        }
    }
}