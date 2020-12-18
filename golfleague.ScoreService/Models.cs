using System;

namespace golfleague.ScoreService
{
    public class Score
    {
        public DateTime ScoreDate { get; set; }
        public string GolferName { get; set; }
        public string GolferId { get; set; }
        public string GrossScore { get; set; }
        public string NetScore { get; set; }
        public float OriginalHandicap { get; set; }
        public float NewHandicap { get; set; }
        public string ScoreId { get; set; }
    }
}