using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace matchmaking.data.Models
{
    public class Match
    {
        public Match(IList<Player> players)
        {
            var dtNow = DateTime.UtcNow;

            NumberOfPlayers = players.Count();

            MinSkill = players.Min(x => x.Skill);
            MaxSkill = players.Max(x => x.Skill);
            AverageSkill = Math.Round(players.Average(x => x.Skill), 3);

            MinRemoteness = players.Min(x => x.Remoteness);
            MaxRemoteness = players.Max(x => x.Remoteness);
            AverageRemoteness = players.Average(x => x.Remoteness);

            MinWait = Math.Round(players.Min(x => (dtNow - x.DateTimeAdded).TotalSeconds),0);
            MaxWait = Math.Round(players.Max(x => (dtNow - x.DateTimeAdded).TotalSeconds), 0);
            AvgWait = Math.Round(players.Average(x => (dtNow - x.DateTimeAdded).TotalSeconds), 0);

            DateTimeStarted = dtNow;
        }

        public Match()
        {

        }


        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public int NumberOfPlayers { get; set; }
        [Required]
        public double MinSkill { get; set; }
        [Required]
        public double MaxSkill { get; set; }
        [Required]
        public double AverageSkill { get; set; }
        [Required]
        public int MinRemoteness { get; set; }
        [Required]
        public int MaxRemoteness { get; set; }
        [Required]
        public double AverageRemoteness { get; set; }
        [Required]
        public double MinWait { get; set; }
        [Required]
        public double MaxWait { get; set; }
        [Required]
        public double AvgWait { get; set; }
        [Required]
        public DateTime DateTimeStarted { get; set; }

        public override string ToString()
        {
            return $"#{Id}\t{NumberOfPlayers} players\tSk:({MinSkill}<>{MaxSkill}|{AverageSkill})\t" +
                $"Re:({MinRemoteness}<>{MaxRemoteness}|{AverageRemoteness})\t" + 
                $"Wt:({MinWait}<>{MaxWait}|{AvgWait})";
        }
    }
}
