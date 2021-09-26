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
using HDC.MonoBehaviours;

namespace HDC.Cards
{
    class DivineBlessing : CustomCard
    {
        private float block_cooldown = 0.25f;
        private float health_boost = 0.50f;
        private float health_restore = 0.15f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var block = gameObject.AddComponent<Block>();
            block.InvokeMethod("ResetStats");
            block.cdMultiplier = 1-block_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            DivineBlessing_Effect divineBlessing = player.gameObject.AddComponent<DivineBlessing_Effect>();
            divineBlessing.player = player;
            divineBlessing.data = data;
            divineBlessing.block = block;
            divineBlessing.healRatio = health_restore;
            data.maxHealth *= (1+health_boost);
        }      
        public override void OnRemoveCard()
        {
         
        }
        protected override string GetTitle()
        {
            return "Divine Blessing";
        }
        protected override string GetDescription()
        {
            return $"Heal {health_restore*100}% of your hp everytime you block";
        }
        protected override GameObject GetCardArt()
        {
            try
            {
                return HDC.ArtAssets.LoadAsset<GameObject>("C_AngelCard");
            }
            catch
            {
                UnityEngine.Debug.Log("Something went wrong with card art");
                return null;
            }
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                HDC.FormatStat(true,"Block Cooldown",-block_cooldown,CardInfoStat.SimpleAmount.lower),
                HDC.FormatStat(true,"Health",health_boost,CardInfoStat.SimpleAmount.Some)
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
