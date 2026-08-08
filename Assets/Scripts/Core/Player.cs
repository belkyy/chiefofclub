using System;

namespace FootballManager.Core
{
    [Serializable]
    public class Player
    {
        public string Name;
        public int Age;
        public Position MainPosition;
        public int Value; // Sterlin cinsinden piyasa değeri
        public int Wage;  // Haftalık maaş

        // Dynamic & Hidden Stats (Ezberlenemez yapı için)
        public int Potential;     // Maximum ulaşabileceği güç
        public int Consistency;   // İstikrar (0-100) -> Maç içi form dalgalanmasını etkiler
        public int Form;          // Güncel form durumu (0-100)
        public int Moral;         // Moral durumu (0-100)

        // General & Technical Stats (0-100)
        public int Pace;          // Hız
        public int Shooting;      // Bitiricilik / Şut
        public int Passing;       // Pas
        public int Dribbling;     // Top Kontrolü / Dribling
        public int Defending;     // Savunma / Müdahale
        public int Physical;      // Güç / Dayanıklılık

        // Goalkeeper Specific Stats
        public int Reflexes;      // Refleks
        public int Positioning;   // Pozisyon Alma

        // Oyuncunun Mevcut Genel Gücü (Overall Score)
        public int GetOverall()
        {
            if (MainPosition == Position.GK)
            {
                return (int)((Reflexes * 0.4f) + (Positioning * 0.4f) + (Passing * 0.2f));
            }

            return (int)((Pace * 0.15f) + (Shooting * 0.2f) + (Passing * 0.2f) +
                         (Dribbling * 0.15f) + (Defending * 0.15f) + (Physical * 0.15f));
        }
    }
}