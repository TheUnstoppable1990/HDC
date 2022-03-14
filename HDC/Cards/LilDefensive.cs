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
        private float health_boost = 0.50f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //block.InvokeMethod("ResetStats");
            block.cdMultiplier = 1f - block_cooldown;
            statModifiers.health = 1f + health_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

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
                CardTools.FormatStat(true,"Health",health_boost)
            };
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Lil_Defensive");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
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
