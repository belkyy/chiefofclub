using System;
using System.Collections.Generic;
using UnityEngine;
using FootballManager.Core;

namespace FootballManager.Generators
{
    public class WorldGenerator : MonoBehaviour
    {
        private static readonly string[] FirstNames = { "Kaio", "Mert", "Lucas", "Enzo", "Mateo", "Santi", "Arthur", "Leo" };
        private static readonly string[] LastNames = { "Kylia", "Çapkın", "Silva", "Ramos", "Rossi", "Santos", "Novak", "Kroos" };

        // Seed sayesinde her server/save için farklı bir evren üretilir
        public static Team CreateRandomTeam(string teamName, int tier, int seed)
        {
            System.Random random = new System.Random(seed);
            Team team = new Team
            {
                Name = teamName,
                Tier = tier,
                Budget = (5 - tier) * 10000000L, // Tier 1 -> 40M, Tier 3 -> 20M
                Reputation = (5 - tier) * 20
            };

            // Kadroya 11 oyuncu üretelim
            Position[] positions = {
                Position.GK, Position.LB, Position.CB, Position.CB, Position.RB,
                Position.CM, Position.CM, Position.CAM, Position.LW, Position.RW, Position.ST
            };

            foreach (var pos in positions)
            {
                Player p = CreateRandomPlayer(pos, tier, random);
                team.Squad.Add(p);
            }

            return team;
        }

        public static Player CreateRandomPlayer(Position pos, int teamTier, System.Random rand)
        {
            // Tier 1 takımların oyuncuları 75-90 arası, Tier 3 takımların 60-75 arası stat alır
            int baseMin = 90 - (teamTier * 10);
            int baseMax = 98 - (teamTier * 8);

            Player p = new Player
            {
                Name = FirstNames[rand.Next(FirstNames.Length)] + " " + LastNames[rand.Next(LastNames.Length)],
                Age = rand.Next(18, 34),
                MainPosition = pos,
                Consistency = rand.Next(40, 99), // Gizli İstikrar
                Potential = rand.Next(baseMin, 99), // Tavan potansiyel
                Form = rand.Next(70, 100),
                Moral = rand.Next(70, 100)
            };

            // Mevkisine göre baskın statlar oluşturulur
            p.Pace = rand.Next(baseMin - 10, baseMax);
            p.Passing = rand.Next(baseMin - 10, baseMax);
            p.Shooting = (pos == Position.ST || pos == Position.CAM) ? rand.Next(baseMin, baseMax) : rand.Next(40, 70);
            p.Defending = (pos == Position.CB || pos == Position.LB || pos == Position.RB) ? rand.Next(baseMin, baseMax) : rand.Next(30, 65);
            p.Dribbling = rand.Next(baseMin - 10, baseMax);
            p.Physical = rand.Next(baseMin - 10, baseMax);

            if (pos == Position.GK)
            {
                p.Reflexes = rand.Next(baseMin, baseMax);
                p.Positioning = rand.Next(baseMin, baseMax);
            }

            // Piyasa değeri genel gücüne ve yaşına göre hesaplanır
            p.Value = p.GetOverall() * 500000 * (35 - p.Age) / 10;
            p.Wage = p.Value / 100;

            return p;
        }
    }
}