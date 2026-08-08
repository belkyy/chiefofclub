using System.Collections.Generic;

namespace FootballManager.Core
{
    [System.Serializable]
    public class Team
    {
        public string Name;
        public int Tier; // Tier 1: Real Madrid/Galatasaray dengi, Tier 3: Vasat
        public long Budget; // Transfer Bütçesi
        public int Reputation; // Takım İtibarı (1-100)

        public Tactics CurrentTactics = new Tactics();
        public List<Player> Squad = new List<Player>();

        public int GetTeamOverall()
        {
            if (Squad == null || Squad.Count == 0) return 0;

            int total = 0;
            foreach (var player in Squad)
            {
                total += player.GetOverall();
            }
            return total / Squad.Count;
        }
    }

}