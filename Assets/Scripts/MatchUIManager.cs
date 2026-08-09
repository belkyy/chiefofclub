using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FootballManager.Core;
using FootballManager.Generators;

namespace FootballManager
{
    public class MatchUIManager : MonoBehaviour
    {
        [Header("Skor & Spiker UI")]
        public Text homeTeamText;
        public Text scoreText;
        public Text awayTeamText;
        public Text minuteText;
        public Text commentaryText;
        public Button startMatchButton;

        [Header("2D Saha UI")]
        public RectTransform ballMarker;
        public float ballMoveSpeed = 6f;

        [Header("Oyuncu Panelleri")]
        public Transform onFieldListParent;
        public Transform subsListParent;
        public GameObject playerButtonPrefab;

        [Header("Taktik & Diziliş UI")]
        public Dropdown outOfPossessionDropdown;
        public Dropdown inPossessionDropdown;
        public Dropdown formationDropdown;

        private Team homeTeam;
        private Team awayTeam;
        private MatchResult currentMatchResult;

        private Player selectedSubPlayer = null;
        private Player selectedOnFieldPlayer = null;

        private Vector2 targetBallPosition;
        private readonly Vector2 posHomeGoal = new Vector2(-400f, 0f);
        private readonly Vector2 posMidfield = new Vector2(0f, 0f);
        private readonly Vector2 posAwayGoal = new Vector2(400f, 0f);

        void Start()
        {
            startMatchButton.onClick.AddListener(StartMatch);

            homeTeam = WorldGenerator.CreateRandomTeam("Galata FC", 1, 101);
            awayTeam = WorldGenerator.CreateRandomTeam("Bosphorus SK", 1, 202);

            while (homeTeam.Squad.Count < 18)
            {
                Player subPlayer = WorldGenerator.CreateRandomTeam("Temp", 1, UnityEngine.Random.Range(100, 999)).Squad[0];
                subPlayer.Name = "Yedek " + (homeTeam.Squad.Count - 10);
                homeTeam.Squad.Add(subPlayer);
            }

            // Başlangıç statlarını sıfırla
            ResetAllPlayerStats();

            homeTeamText.text = homeTeam.Name;
            scoreText.text = "0 - 0";
            awayTeamText.text = awayTeam.Name;
            minuteText.text = "00'";
            commentaryText.text = "Maçın başlaması bekleniyor...";

            targetBallPosition = posMidfield;
            if (ballMarker != null) ballMarker.anchoredPosition = posMidfield;

            SetupTacticsAndFormations();
            RefreshPlayerLists();
        }

        void Update()
        {
            if (ballMarker != null)
            {
                ballMarker.anchoredPosition = Vector2.Lerp(
                    ballMarker.anchoredPosition,
                    targetBallPosition,
                    Time.deltaTime * ballMoveSpeed
                );
            }
        }

        private void ResetAllPlayerStats()
        {
            foreach (var p in homeTeam.Squad)
            {
                p.Stamina = 100f;
                p.MatchRating = 6.0f;
                p.IsInjured = false;
            }
            foreach (var p in awayTeam.Squad)
            {
                p.Stamina = 100f;
                p.MatchRating = 6.0f;
                p.IsInjured = false;
            }
        }

        private void SetupTacticsAndFormations()
        {
            if (outOfPossessionDropdown != null)
            {
                outOfPossessionDropdown.ClearOptions();
                outOfPossessionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(OutOfPossessionStyle))));
            }

            if (inPossessionDropdown != null)
            {
                inPossessionDropdown.ClearOptions();
                inPossessionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(InPossessionStyle))));
            }

            if (formationDropdown != null)
            {
                formationDropdown.ClearOptions();
                formationDropdown.AddOptions(new List<string> { "4-2-3-1", "3-5-2", "4-4-2", "4-3-3" });
                formationDropdown.onValueChanged.AddListener(OnFormationChanged);
            }
        }

        private void OnFormationChanged(int index)
        {
            Position[] newPositions;
            switch (index)
            {
                case 1: // 3-5-2
                    newPositions = new Position[] { Position.GK, Position.CB, Position.CB, Position.CB, Position.LM, Position.CM, Position.CM, Position.RM, Position.CAM, Position.ST, Position.ST };
                    break;
                case 2: // 4-4-2
                    newPositions = new Position[] { Position.GK, Position.LB, Position.CB, Position.CB, Position.RB, Position.LM, Position.CM, Position.CM, Position.RM, Position.ST, Position.ST };
                    break;
                case 3: // 4-3-3
                    newPositions = new Position[] { Position.GK, Position.LB, Position.CB, Position.CB, Position.RB, Position.CM, Position.CM, Position.CM, Position.LW, Position.RW, Position.ST };
                    break;
                default: // 4-2-3-1
                    newPositions = new Position[] { Position.GK, Position.LB, Position.CB, Position.CB, Position.RB, Position.CDM, Position.CDM, Position.CAM, Position.LW, Position.RW, Position.ST };
                    break;
            }

            for (int i = 0; i < 11 && i < homeTeam.Squad.Count; i++)
            {
                homeTeam.Squad[i].MainPosition = newPositions[i];
            }

            RefreshPlayerLists();
        }

        public void RefreshPlayerLists()
        {
            if (onFieldListParent == null || subsListParent == null) return;

            foreach (Transform child in onFieldListParent) Destroy(child.gameObject);
            foreach (Transform child in subsListParent) Destroy(child.gameObject);

            for (int i = 0; i < homeTeam.Squad.Count; i++)
            {
                Player p = homeTeam.Squad[i];
                Transform parent = (i < 11) ? onFieldListParent : subsListParent;

                GameObject btnObj = Instantiate(playerButtonPrefab, parent);
                Text btnText = btnObj.GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.supportRichText = true;
                    btnText.text = $"{GetStaminaBarHTML(p.Stamina)} {p.MainPosition} | {p.Name} {GetRatingHTML(p.MatchRating)}";
                }

                Button btn = btnObj.GetComponent<Button>();
                bool isOnField = (i < 11);
                btn.onClick.AddListener(() => OnPlayerSelected(p, isOnField));
            }
        }

        private string GetStaminaBarHTML(float stamina)
        {
            if (stamina > 80f) return "<color=#006400>[█████]</color>"; // Koyu Yeşil
            if (stamina > 60f) return "<color=#00FF00>[████░]</color>"; // Yeşil
            if (stamina > 40f) return "<color=#FFFF00>[███░░]</color>"; // Sarı
            if (stamina > 20f) return "<color=#FFA500>[██░░░]</color>"; // Turuncu
            return "<color=#FF0000>[█░░░░]</color>";                     // Kırmızı
        }

        private string GetRatingHTML(float rating)
        {
            string colorHex = "#FFD700"; // Sarı (6.0 - 6.9)

            if (rating >= 9.0f) colorHex = "#FF69B4";      // Pembe
            else if (rating >= 8.0f) colorHex = "#1E90FF"; // Mavi
            else if (rating >= 7.0f) colorHex = "#008000"; // Koyu Yeşil
            else if (rating >= 6.0f) colorHex = "#FFD700"; // Sarı
            else if (rating >= 5.0f) colorHex = "#FF0000"; // Kırmızı
            else colorHex = "#8B0000";                     // Koyu Kırmızı

            return $"<color={colorHex}>({rating:F1})</color>";
        }

        private void OnPlayerSelected(Player player, bool isOnField)
        {
            if (isOnField) selectedOnFieldPlayer = player;
            else selectedSubPlayer = player;

            if (selectedOnFieldPlayer != null && selectedSubPlayer != null)
            {
                ExecuteSubstitution(selectedOnFieldPlayer, selectedSubPlayer);
            }
        }

        private void ExecuteSubstitution(Player outPlayer, Player inPlayer)
        {
            int outIndex = homeTeam.Squad.IndexOf(outPlayer);
            int inIndex = homeTeam.Squad.IndexOf(inPlayer);

            if (outIndex != -1 && inIndex != -1)
            {
                Position tempPos = outPlayer.MainPosition;
                outPlayer.MainPosition = inPlayer.MainPosition;
                inPlayer.MainPosition = tempPos;

                homeTeam.Squad[outIndex] = inPlayer;
                homeTeam.Squad[inIndex] = outPlayer;

                commentaryText.text = $"<b>OYUNCU DEĞİŞİKLİĞİ:</b> {outPlayer.Name} ➔ {inPlayer.Name}";
            }

            selectedOnFieldPlayer = null;
            selectedSubPlayer = null;

            RefreshPlayerLists();
        }

        void StartMatch()
        {
            startMatchButton.interactable = false;
            ResetAllPlayerStats();
            currentMatchResult = MatchEngine.SimulateMatch(homeTeam, awayTeam, UnityEngine.Random.Range(1, 9999));
            StartCoroutine(PlayMatchFlow());
        }

        IEnumerator PlayMatchFlow()
        {
            int logIndex = 0;

            for (int minute = 1; minute <= 90; minute++)
            {
                minuteText.text = $"{minute:00}'";

                // 1. DAKİKALIK STAMİNA DÜŞÜŞÜ (Sadece sahadaki 11 yorulur)
                for (int i = 0; i < 11 && i < homeTeam.Squad.Count; i++)
                {
                    Player p = homeTeam.Squad[i];
                    p.Stamina = Mathf.Max(0f, p.Stamina - UnityEngine.Random.Range(0.35f, 0.5f)); // 90dk sonunda ~%60 kalır
                }

                // 2. OLAY KONTROLÜ VE REYTING GÜNCELLEMESİ
                if (logIndex < currentMatchResult.Commentary.Count &&
                    currentMatchResult.Commentary[logIndex].Minute == minute)
                {
                    MatchLog currentLog = currentMatchResult.Commentary[logIndex];
                    scoreText.text = currentLog.CurrentScore;
                    commentaryText.text = currentLog.Text;

                    targetBallPosition = currentLog.IsHomeAttack ? posAwayGoal : posHomeGoal;

                    // Olay tipine göre anlık reyting güncellemeleri
                    if (currentLog.EventType == MatchEventType.Goal && currentLog.Attacker != null)
                    {
                        currentLog.Attacker.MatchRating = Mathf.Clamp(currentLog.Attacker.MatchRating + 0.8f, 4.0f, 9.9f);
                        if (currentLog.Goalkeeper != null)
                            currentLog.Goalkeeper.MatchRating = Mathf.Clamp(currentLog.Goalkeeper.MatchRating - 0.3f, 4.0f, 9.9f);
                    }
                    else if (currentLog.EventType == MatchEventType.Save && currentLog.Goalkeeper != null)
                    {
                        currentLog.Goalkeeper.MatchRating = Mathf.Clamp(currentLog.Goalkeeper.MatchRating + 0.3f, 4.0f, 9.9f);
                        if (currentLog.Attacker != null)
                            currentLog.Attacker.MatchRating = Mathf.Clamp(currentLog.Attacker.MatchRating - 0.1f, 4.0f, 9.9f);
                    }

                    logIndex++;
                    RefreshPlayerLists(); // Ekranı anlık güncelle
                    yield return new WaitForSeconds(1.5f);
                }
                else
                {
                    targetBallPosition = posMidfield;
                    RefreshPlayerLists(); // Kondisyon düşüşünü ekrana yansıt
                    yield return new WaitForSeconds(0.08f);
                }
            }

            minuteText.text = "90' (M.S.)";
            commentaryText.text = $"MAÇ BİTTİ! Sonuç: {homeTeam.Name} {currentMatchResult.HomeGoals} - {currentMatchResult.AwayGoals} {awayTeam.Name}";
            targetBallPosition = posMidfield;
            RefreshPlayerLists();
            startMatchButton.interactable = true;
        }
    }
}