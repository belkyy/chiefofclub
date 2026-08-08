using System;
using System.Collections.Generic;
using UnityEngine;

namespace FootballManager.Core
{
    [Serializable]
    public class MatchLog
    {
        public int Minute;
        public string Text;
        public string CurrentScore;
    }

    public class MatchResult
    {
        public Team HomeTeam;
        public Team AwayTeam;
        public int HomeGoals;
        public int AwayGoals;
        public List<MatchLog> Commentary = new List<MatchLog>();
    }

    public static class MatchEngine
    {
        public static MatchResult SimulateMatch(Team home, Team away, int seed)
        {
            System.Random rand = new System.Random(seed);
            MatchResult result = new MatchResult { HomeTeam = home, AwayTeam = away };

            int homeGoals = 0;
            int awayGoals = 0;

            for (int minute = 1; minute <= 90; minute++)
            {
                // Taktiksel Mentelite katsayıları
                float homeAttackModifier = GetMentalityAttackBonus(home.CurrentTactics.TeamMentality);
                float awayAttackModifier = GetMentalityAttackBonus(away.CurrentTactics.TeamMentality);

                if (rand.Next(0, 100) < 30) // Atak İhtimali
                {
                    // Takım güçleri + Taktik modifier hesaplanır
                    float homePower = home.GetTeamOverall() * homeAttackModifier;
                    float awayPower = away.GetTeamOverall() * awayAttackModifier;

                    bool isHomeAttacking = rand.Next(0, (int)(homePower + awayPower)) < homePower;
                    Team attackingTeam = isHomeAttacking ? home : away;
                    Team defendingTeam = isHomeAttacking ? away : home;

                    Player attacker = attackingTeam.Squad.Find(p => p.MainPosition == Position.ST) ?? attackingTeam.Squad[0];
                    Player goalkeeper = defendingTeam.Squad.Find(p => p.MainPosition == Position.GK) ?? defendingTeam.Squad[0];

                    // High Press veya Offside Trap Kontrolü
                    if (defendingTeam.CurrentTactics.OffsideTrap && rand.Next(0, 100) < 40)
                    {
                        result.Commentary.Add(new MatchLog
                        {
                            Minute = minute,
                            Text = $"{attackingTeam.Name} atağa kalktı fakat {defendingTeam.Name} defansı harika zamanlamayla **OFSAYT** tuzağına düşürdü!",
                            CurrentScore = $"{homeGoals} - {awayGoals}"
                        });
                        continue;
                    }

                    
                    int attackPower = (int)(attacker.Shooting * Mathf.Clamp(homeAttackModifier, 0.8f, 1.15f)) + (attacker.Form / 10) + rand.Next(-10, 10);
                    int defensePower = goalkeeper.Reflexes + (goalkeeper.Form / 10) + rand.Next(-10, 10);

                    if (attackPower > defensePower)
                    {
                        if (isHomeAttacking) homeGoals++; else awayGoals++;

                        result.Commentary.Add(new MatchLog
                        {
                            Minute = minute,
                            Text = $"<b>GOOOLL!</b> {attacker.Name} taktiğin meyvesini topluyor ve ağları havalandırıyor!",
                            CurrentScore = $"{homeGoals} - {awayGoals}"
                        });
                    }
                    else
                    {
                        result.Commentary.Add(new MatchLog
                        {
                            Minute = minute,
                            Text = $"{attacker.Name} şutunu çekti ama kaleci {goalkeeper.Name} başarılı.",
                            CurrentScore = $"{homeGoals} - {awayGoals}"
                        });
                    }
                }
            }

            result.HomeGoals = homeGoals;
            result.AwayGoals = awayGoals;
            return result;
        }

        private static float GetMentalityAttackBonus(Mentality mentality)
        {
            switch (mentality)
            {
                case Mentality.ParkTheBus: return 0.6f;
                case Mentality.Defensive: return 0.8f;
                case Mentality.Balanced: return 1.0f;
                case Mentality.Attacking: return 1.2f;
                case Mentality.AllOutAttack: return 1.4f;
                default: return 1.0f;
            }
        }
    }
}