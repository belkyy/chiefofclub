using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FootballManager.Core;

namespace FootballManager.UI
{
    public class TransferUIController : MonoBehaviour
    {
        [Header("UI Elemanları")]
        public Text budgetText;
        public Transform playerListParent;   // ScrollView / Content nesnesi
        public GameObject playerCardPrefab; // Oyuncu satır prefab'ı

        void OnEnable()
        {
            // Transfer Paneli her aktif olduğunda listeyi yenile
            TransferMarket.GenerateInitialMarket();
            RefreshTransferUI();
        }

        public void RefreshTransferUI()
        {
            budgetText.text = $"Kulüp Bütçesi: {TransferMarket.UserBudget:N0} €";

            // Eski kartları temizle
            foreach (Transform child in playerListParent)
            {
                Destroy(child.gameObject);
            }

            // Pazardaki oyuncuları listele
            foreach (var item in TransferMarket.AvailablePlayers)
            {
                GameObject card = Instantiate(playerCardPrefab, playerListParent);

                // Kart üzerindeki metinleri doldur
                Text cardText = card.GetComponentInChildren<Text>();
                if (cardText != null)
                {
                    int avgPower = (item.PlayerData.Pace + item.PlayerData.Shooting + item.PlayerData.Passing) / 3;
                    cardText.text = $"{item.PlayerData.MainPosition} | <b>{item.PlayerData.Name}</b> (Güç: {avgPower}) - Kulüp: {item.SellerTeamName} | <color=green>{item.Price:N0} €</color>";
                }

                Button buyBtn = card.GetComponentInChildren<Button>();
                if (buyBtn != null)
                {
                    buyBtn.onClick.AddListener(() => BuyPlayer(item));
                }
            }
        }

        private void BuyPlayer(TransferMarketItem item)
        {
            if (TransferMarket.UserBudget >= item.Price)
            {
                TransferMarket.UserBudget -= item.Price;
                TransferMarket.AvailablePlayers.Remove(item);

                Debug.Log($"{item.PlayerData.Name} başarıyla transfer edildi!");

                RefreshTransferUI();
            }
            else
            {
                Debug.Log("Bütçe Yetersiz!");
            }
        }
    }
}