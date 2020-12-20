namespace golfleague.RosterService
{
    using System.Collections.Generic;
    using System;
    public class Golfer
    {
        public string Name { get; set; }
        public string GolferId { get; set; }
        public string Email { get; set; }
        public IEnumerable<Score> Scores { get; set; }
    }

    public class Score
    {
        public DateTime ScoreDate { get; set; }
        public int GrossScore { get; set; }

        public int NetScore { get; set; }

        public float Handicap { get; set; }
    }
}