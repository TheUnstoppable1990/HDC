using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using HDC.MonoBehaviours;
using HDC.Utilities;

namespace HDC.Cards
{
    class PointBlank : CustomCard
    {
        private int bullets = 5;
        private float damage = 1f;
        private int ammo = 5;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.numberOfProjectiles = bullets;
            gun.damage = 1f+damage;
            gun.destroyBulletAfter = 0.1f;
            gun.spread = 0f;
            gun.ammo = this.ammo;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
        }
        protected override string GetTitle()
        {
            return "Point Blank";
        }
        protected override string GetDescription()
        {
            return "Warning, will shorten your range to nothing and increase your damage to everything.";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Point_Blank");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Damage",damage),
                CardTools.FormatStat(true,"Bullets",bullets),
                CardTools.FormatStat(true,"Ammo",ammo)
            };
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
