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
using HDC.Utilities;

namespace HDC.Cards
{
    class DivineBlessing : CustomCard
    {
        private float block_cooldown = 0.25f;
        private float health_boost = 0.50f;
        private float health_restore = 0.1f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            block.InvokeMethod("ResetStats");
            block.cdMultiplier = 1-block_cooldown;
            statModifiers.health = 1f + health_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            DivineBlessing_Effect divineBlessing = player.gameObject.GetOrAddComponent<DivineBlessing_Effect>();
            divineBlessing.player = player;
            divineBlessing.data = data;
            divineBlessing.block = block;
            divineBlessing.healRatio = health_restore;
        }      
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
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
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Divine_Blessing");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Block Cooldown",-block_cooldown),
                CardTools.FormatStat(true,"Health",health_boost)
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
