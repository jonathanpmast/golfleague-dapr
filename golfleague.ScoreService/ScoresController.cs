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

namespace golfleague.ScoreService
{
    [ApiController]
    [Route("[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly string daprPort, stateStoreName, pubsubName;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScoresController> _logger;

        private readonly IConfiguration _configuration;
        public ScoresController(IHttpClientFactory httpClientFactory,
                                ILogger<ScoresController> logger,
                                IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;

            daprPort = _configuration["DAPR_HTTP_PORT"];
            stateStoreName = _configuration["DaprComponentNames:statestore.local"];
            pubsubName = _configuration["DaprComponentNames:pubsub.GolfLeague"];
        }

        [HttpDelete("[controller]/{golferId}/{scoreId}")]
        public async Task<IActionResult> RemoveScore(string golferId, string scoreId)
        {
            var scores = await GetScores(golferId);
            var scoresList = scores.ToList();
            scoresList.RemoveAll(s => s.ScoreId == scoreId);
            return (await SaveScoreList(scoresList)) ? Ok() : BadRequest();
        }
        [HttpPost("[controller]")]
        public async Task<IActionResult> NewScore(CloudEvent cloudEvent)
        {
            var score = ((JToken)cloudEvent.Data).ToObject<Score>();
            _logger.LogInformation("Received Score: {@Score}", score);

            var scores = await GetScores(score.GolferId);
            var scoresList = scores.ToList();
            scoresList.Add(score);

            var saveResult = await SaveScoreList(scoresList);
            if (saveResult)
                PublishNewScore(score);
            else
                return this.BadRequest();

            return Ok();
        }

        private async void PublishNewScore(Score score)
        {
            var url = $"http://localhost:{daprPort}/v1.0/publish/{pubsubName}/scores";
            var json = JsonConvert.SerializeObject(score);
            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Score succesfully published");
            else
                _logger.LogInformation("Failed to publish score");
        }

        private async Task<bool> SaveScoreList(IEnumerable<Score> scores)
        {
            var url = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}";

            var data = new[] { new { key = scores.First().GolferId, value = scores } };
            var json = JsonConvert.SerializeObject(data);

            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, httpContent);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Scores succesfully saved");
            else
            {
                _logger.LogInformation($"Score failed to save: {@response.StatusCode} \n {@response.Content}");
                return false;
            }
            return true;
        }


        [HttpGet("[controller]/{golferId}")]
        public async Task<IEnumerable<Score>> GetScoresByGolfer(string golferId)
        {
            return await GetScores(golferId);
        }

        private async Task<IEnumerable<Score>> GetScores(string golferId)
        {

            string URL = $"http://localhost:{daprPort}/v1.0/state/{stateStoreName}/{golferId}";

            var response = await _httpClient.GetAsync(URL);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _logger.LogInformation("No scores stored for given golfer");
                return new List<Score>();
            }

            var data = await response.Content.ReadAsStringAsync();
            var scores = JsonConvert.DeserializeObject<IEnumerable<Score>>(data);
            return scores;
        }


    }
}