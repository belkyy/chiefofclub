using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FootballManager.Core;
using FootballManager.Generators;

namespace FootballManager.Core
{
    public class TransferMarketItem
    {
        public Player PlayerData;
        public int Price;
        public string SellerTeamName;
    }

    public static class TransferMarket
    {
        public static List<TransferMarketItem> AvailablePlayers = new List<TransferMarketItem>();
        public static int UserBudget = 25000000; // Başlangıç Bütçesi: 25M €

        public static void GenerateInitialMarket()
        {
            if (AvailablePlayers.Count > 0) return; // Zaten üretildiyse tekrar üretme

            string[] dummyTeams = { "Madrid FC", "London Blue", "Milano Red", "Munich Star", "Paris SG" };

            for (int i = 0; i < 15; i++)
            {
                Team tempTeam = WorldGenerator.CreateRandomTeam(dummyTeams[Random.Range(0, dummyTeams.Length)], 1, Random.Range(1000, 9999));
                Player randomPlayer = tempTeam.Squad[Random.Range(0, tempTeam.Squad.Count)];

                // Oyuncu gücüne göre tahmini fiyat hesabı (Örn: OVR 75 -> 7.5M €)
                int basePower = (randomPlayer.Pace + randomPlayer.Shooting + randomPlayer.Passing) / 3;
                int price = basePower * 150000;

                AvailablePlayers.Add(new TransferMarketItem
                {
                    PlayerData = randomPlayer,
                    Price = price,
                    SellerTeamName = tempTeam.Name
                });
            }
        }
    }
}