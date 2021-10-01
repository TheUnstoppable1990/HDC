using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using HDC.MonoBehaviours;
namespace HDC.Cards
{
    class HolyLight : CustomCard
    {
        private HolyLight_Effect holyLight_effect;
        private Player player;
        private float health_boost = 0.25f;
        private float damage_ratio = 1f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            this.holyLight_effect = player.gameObject.AddComponent<HolyLight_Effect>();
            this.holyLight_effect.player = player;
            this.holyLight_effect.damageRatio = this.damage_ratio;
            this.holyLight_effect.block = block;
            this.player = player;
            data.maxHealth *= (1f + health_boost);
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
            Destroy(holyLight_effect);
        }
        protected override string GetTitle()
        {
            return "Holy Light";
        }
        protected override string GetDescription()
        {
            return "Your holy light builds by every bit of health you regain. When you block, you unleash this light in a devestating AOE.";
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
                HDC.FormatStat(true,"Health",health_boost,CardInfoStat.SimpleAmount.aHugeAmountOf),
                HDC.FormatStat(true,"Heal Damage on Discharge ",damage_ratio,CardInfoStat.SimpleAmount.aLotOf)
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
