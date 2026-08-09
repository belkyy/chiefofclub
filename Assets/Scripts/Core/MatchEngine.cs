using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FootballManager.Core
{
    public enum InPossessionStyle { PasliOyna, KontrayaKalk, Gegenpress, TikiTaka, BolSut }
    public enum OutOfPossessionStyle { KarsiPres, GerideKal, OtobusuCek, YuksekPres }
    public enum FormationType { F4231, F352, F442, F433 }
    public enum MatchEventType { None, Goal, Save, Offside }

    [Serializable]
    public class MatchLog
    {
        public int Minute;
        public string Text;
        public string CurrentScore;
        public bool IsHomeAttack;
        public MatchEventType EventType = MatchEventType.None;
        public Player Attacker;
        public Player Goalkeeper;
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
                if (rand.Next(0, 100) < 25) // %25 Atak ihtimali
                {
                    bool isHomeAttacking = rand.Next(0, 100) < 50;
                    Team attackingTeam = isHomeAttacking ? home : away;
                    Team defendingTeam = isHomeAttacking ? away : home;

                    Player attacker = attackingTeam.Squad.Find(p => p.MainPosition == Position.ST) ?? attackingTeam.Squad[0];
                    Player goalkeeper = defendingTeam.Squad[0];

                    int attackPower = attacker.Shooting + rand.Next(-10, 10);
                    int defensePower = goalkeeper.Reflexes + rand.Next(-10, 10);

                    if (attackPower > defensePower)
                    {
                        if (isHomeAttacking) homeGoals++; else awayGoals++;

                        result.Commentary.Add(new MatchLog
                        {
                            Minute = minute,
                            Text = $"<b>GOOOLL!</b> {attacker.Name} harika bir şutla topu ağlara gönderdi!",
                            CurrentScore = $"{homeGoals} - {awayGoals}",
                            IsHomeAttack = isHomeAttacking,
                            EventType = MatchEventType.Goal,
                            Attacker = attacker,
                            Goalkeeper = goalkeeper
                        });
                    }
                    else
                    {
                        result.Commentary.Add(new MatchLog
                        {
                            Minute = minute,
                            Text = $"{attacker.Name} şutunu çekti ama kaleci {goalkeeper.Name} geçit vermedi.",
                            CurrentScore = $"{homeGoals} - {awayGoals}",
                            IsHomeAttack = isHomeAttacking,
                            EventType = MatchEventType.Save,
                            Attacker = attacker,
                            Goalkeeper = goalkeeper
                        });
                    }
                }
            }

            result.HomeGoals = homeGoals;
            result.AwayGoals = awayGoals;
            return result;
        }
    }
}