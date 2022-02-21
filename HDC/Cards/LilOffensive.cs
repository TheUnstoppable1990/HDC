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
    class LilOffensive : CustomCard
    {
        private float reload_cooldown = 0.35f;
        private int ammo_change = 4;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //block.InvokeMethod("ResetStats");
            gun.reloadTime = 1f - reload_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gunAmmo.maxAmmo += ammo_change;
        }
        public override void OnRemoveCard()
        {
            throw new NotImplementedException();
        }
        protected override string GetTitle()
        {
            return "Lil Offensive";
        }
        protected override string GetDescription()
        {
            return "Watch your language.";
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Ammo",ammo_change),
                CardTools.FormatStat(true,"Reload Time",-reload_cooldown)
            };
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Lil_Offensive");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
