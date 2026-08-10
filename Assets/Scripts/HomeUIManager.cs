using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HomeUIController : MonoBehaviour
{
    [Header("Header Bilgileri")]
    public TMP_Text clubAndManagerText;

    [Header("Sıradaki Maç")]
    public TMP_Text nextMatchTeamText;
    public TMP_Text nextMatchTimeText;
    public Image opponentLogoImage;
    public Button prepareForMatchButton;

    [Header("Tesisler & Finans")]
    public TMP_Text stadiumLevelText;
    public TMP_Text stadiumBonusText;
    public Button upgradeStadiumButton;

    public TMP_Text infraLevelText;
    public TMP_Text infraBonusText;
    public Button upgradeInfraButton;

    public TMP_Text transferBudgetText;
    public TMP_Text wageBudgetText;

    private void Start()
    {
        // Örnek Test Verilerini Yükle
        UpdateHomeUI("Tirabzon", "Mert Çapkın", "Bosphorus SK", "20:00", 1, 150000, 2, 20000000, 7000000);
    }

    public void UpdateHomeUI(string clubName, string managerName, string opponent, string matchTime, int stadiumLv, int stadiumBonus, int infraLv, double transferBudget, double wageBudget)
    {
        clubAndManagerText.text = $"{clubName} - {managerName}";
        nextMatchTeamText.text = $"[ {opponent} ]";
        nextMatchTimeText.text = $"Match Time : {matchTime}";

        stadiumLevelText.text = $"Level of Stadium: {stadiumLv}";
        stadiumBonusText.text = $"Current Bonus: {stadiumBonus:N0} £";

        infraLevelText.text = $"Level of Infrastructure: {infraLv}";

        transferBudgetText.text = $"Transfer Bütçesi; {transferBudget:N0} £";
        wageBudgetText.text = $"Maaş Bütçesi; {wageBudget:N0} £";
    }
}