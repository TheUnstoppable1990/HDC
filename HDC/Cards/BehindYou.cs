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
    class BehindYou : CustomCard
    {

        private float block_cooldown = 0.5f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers,Block block)
        {
            cardInfo.allowMultiple = false;            
            block.cdMultiplier = 1f + block_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
            //BehindYou_Effect behindYou = player.gameObject.AddComponent<BehindYou_Effect>();
            BehindYou_Effect behindYou = player.gameObject.GetOrAddComponent<BehindYou_Effect>();
            behindYou.player = player;
            behindYou.block = block;
            behindYou.data = data;            
        }
        public override void OnRemoveCard()
        {
            
        }
        protected override string GetTitle()
        {
            return "Behind You";
        }
        protected override string GetDescription()
        {
            return "Find yourself behind enemy lines when you block.";
        }
        protected override GameObject GetCardArt()
        {   
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(false,"Block Cooldown",block_cooldown)
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
