using UnityEngine;
using FootballManager.Core;
using FootballManager.Generators;

namespace FootballManager
{
    public class TestManager : MonoBehaviour
    {
        void Start()
        {
            Team galata = WorldGenerator.CreateRandomTeam("Galata FC", 1, 101);
            Team bosphorus = WorldGenerator.CreateRandomTeam("Bosphorus SK", 2, 202);

            // Bosphorus SK'dan bir forvet seçip satın almaya çalışalım
            Player targetPlayer = bosphorus.Squad.Find(p => p.MainPosition == Position.ST);

            Debug.Log($"<color=yellow>=== TRANSFER PAZARLIĞI BAŞLADI ===</color>");
            Debug.Log($"Hedef Oyuncu: <b>{targetPlayer.Name}</b> | Piyasa Değeri: £{targetPlayer.Value:N0}");
            Debug.Log($"Alıcı Bütçe: £{galata.Budget:N0} | Satıcı Bütçe: £{bosphorus.Budget:N0}");

            // Piyasa değerinden biraz düşük teklif verelim (Pazarlık tetiklesin)
            long offerAmount = (long)(targetPlayer.Value * 0.95f);
            Debug.Log($"Galata FC Teklif Yaptı: £{offerAmount:N0}");

            TransferOffer result = TransferManager.EvaluateOffer(galata, bosphorus, targetPlayer, offerAmount);

            if (result.Status == OfferStatus.Accepted)
            {
                Debug.Log("<color=green><b>TEKLİF KABUL EDİLDİ! Transfer gerçekleşti.</b></color>");
            }
            else if (result.Status == OfferStatus.CounterOffer)
            {
                Debug.Log($"<color=orange><b>PAZARLIK:</b> Satıcı teklifi yetersiz buldu. İstenen Fiyat: £{result.CounterAmount:N0}</color>");
            }
            else
            {
                Debug.Log("<color=red><b>TEKLİF REDDEDİLDİ.</b></color>");
            }
        }
    }
}