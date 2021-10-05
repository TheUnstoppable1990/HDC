using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.Networking;
using System.Collections;
using HarmonyLib;
using HDC.MonoBehaviours;
using HDC.Utilities;

namespace HDC.Cards
{
    class LilDefensive : CustomCard
    {
        private float block_cooldown = 0.35f;
        private int ammo_change = 2;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //block.InvokeMethod("ResetStats");
            block.cdMultiplier = 1f - block_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gunAmmo.maxAmmo -= ammo_change;
        }
        public override void OnRemoveCard()
        {
            throw new NotImplementedException();
        }
        protected override string GetTitle()
        {
            return "Lil Defensive";
        }
        protected override string GetDescription()
        {
            return "It was just a joke.";
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Block Cooldown",-block_cooldown),
                CardTools.FormatStat(false,"Ammo",-ammo_change)
            };
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
