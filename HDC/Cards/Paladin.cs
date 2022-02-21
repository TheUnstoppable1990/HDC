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
    class Paladin : CustomCard
    {
        private Paladin_Effect paladin_effect;
        private Player player;
        private float health_boost = 2f;
        private float regen_percentage = 0.15f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            this.paladin_effect = player.gameObject.AddComponent<Paladin_Effect>();
            this.paladin_effect.player = player;
            this.player = player;
            Paladin_Effect[] components = player.gameObject.GetComponents<Paladin_Effect>();
            float num = 0f;
            for (int i = 1; i < components.Length + 1; i++)
            {
                num += Paladin_Effect.baseRange / (float)i;
            }
            foreach (Paladin_Effect paladin_Effect in components)
            {
                paladin_Effect.rangeOfEffect = Paladin_Effect.baseRange * (float)components.Length;
            }
            data.maxHealth *= 1f + this.health_boost;
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
           Destroy(paladin_effect);                
        }
        protected override string GetTitle()
        {
            return "Paladins Endurance";
        }
        protected override string GetDescription()
        {
            return "Regain health for each nearby enemy and heal your allies when they are near.";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Paladins_Endurance");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Max Health",health_boost),
                CardTools.FormatStat(true,"Regen",$"{regen_percentage*100}% per enemy in range")
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
