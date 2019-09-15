using System;
using System.ComponentModel.DataAnnotations;

namespace matchmaking.data.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        public double Skill { get; set; }
        [Required]
        public int Remoteness { get; set; }
        [Required]
        public DateTime DateTimeAdded { get; set; }

        public override string ToString() => $"N: {Name}\tS: {Skill}\tR: {Remoteness}";

        public double GetDistance(Player other)
        {
            double RemotenessDistance = Math.Abs(this.Remoteness - other.Remoteness);
            double SkillDistance = Math.Abs(this.Skill - other.Skill);

            double distance = (SkillDistance * 100) + RemotenessDistance;
            return distance;
        }

        public double GetDistance(double avgSkill, double avgRemoteness)
        {
            double RemotenessDistance = Math.Abs(this.Remoteness - avgRemoteness);
            double SkillDistance = Math.Abs(this.Skill - avgSkill);

            double distance = (SkillDistance * 100) + RemotenessDistance;
            return distance;
        }
    }
}
