using System;
using System.Collections.Generic;
using UnityEngine;

namespace FootballManager.Core
{
    public enum OfferStatus
    {
        Pending,
        Accepted,
        Rejected,
        CounterOffer
    }

    [Serializable]
    public class TransferOffer
    {
        public Player TargetPlayer;
        public Team BuyerTeam;
        public Team SellerTeam;
        public long OfferedAmount;
        public long CounterAmount;
        public OfferStatus Status = OfferStatus.Pending;
    }

    public static class TransferManager
    {
        // Bir takımdan diğerine teklif yapma ve AI değerlendirmesi
        public static TransferOffer EvaluateOffer(Team buyer, Team seller, Player player, long offeredAmount)
        {
            TransferOffer offer = new TransferOffer
            {
                TargetPlayer = player,
                BuyerTeam = buyer,
                SellerTeam = seller,
                OfferedAmount = offeredAmount
            };

            // Alıcının parası yetiyor mu?
            if (buyer.Budget < offeredAmount)
            {
                offer.Status = OfferStatus.Rejected;
                return offer;
            }

            long marketValue = player.Value;

            // AI Karar Mantığı
            if (offeredAmount >= marketValue * 1.2f) // Piyasa değerinin %120 ve üzerine doğrudan KABUL
            {
                offer.Status = OfferStatus.Accepted;
                ExecuteTransfer(offer);
            }
            else if (offeredAmount >= marketValue * 0.9f) // %90 - %120 arası Karşı Teklif (Pazarlık)
            {
                offer.Status = OfferStatus.CounterOffer;
                offer.CounterAmount = (long)(marketValue * 1.25f);
            }
            else // Düşük tekliflere Doğrudan RED
            {
                offer.Status = OfferStatus.Rejected;
            }

            return offer;
        }

        // Transferi Gerçekleştirme ve Parayı/Kadroyu Güncelleme
        public static void ExecuteTransfer(TransferOffer offer)
        {
            offer.BuyerTeam.Budget -= offer.OfferedAmount;
            offer.SellerTeam.Budget += offer.OfferedAmount;

            offer.SellerTeam.Squad.Remove(offer.TargetPlayer);
            offer.BuyerTeam.Squad.Add(offer.TargetPlayer);
        }
    }
}