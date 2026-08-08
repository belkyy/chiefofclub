namespace FootballManager.Core
{
    public enum Formation
    {
        F_433,
        F_4231,
        F_352,
        F_532
    }

    public enum Mentality
    {
        ParkTheBus,    // Otobüsü Çek (%15 Şut bonusu düşer, Savunma +%25)
        Defensive,     // Savunma
        Balanced,      // Dengeli
        Attacking,     // Hücum
        AllOutAttack   // Aşırı Hücum (Şut şansı +%30, Defans zafiyeti +%20)
    }

    public enum PassingStyle
    {
        ShortPass,     // Kısa Pas (İstikrarlı, top kaybı az)
        Direct,        // Dikine Pas
        LongBall       // Uzun Top (Forvetin fiziğine bakar)
    }

    [System.Serializable]
    public class Tactics
    {
        public Formation TeamFormation = Formation.F_433;
        public Mentality TeamMentality = Mentality.Balanced;
        public PassingStyle TeamPassing = PassingStyle.ShortPass;
        public bool OffsideTrap = false; // Ofsayt Taktiği
        public bool HighPress = false;    // Yüksek Press
    }
}