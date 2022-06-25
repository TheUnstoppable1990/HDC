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
        public Paladin_Effect paladin_effect;
        private float health_boost = 0.5f;
        //static public float regen_mult = 0.1f;
        static public float regen_percentage = 0.1f;
        static public int regen_amount = 10;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Saint.HolyClass };

            statModifiers.health = 1f + this.health_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            this.paladin_effect = player.gameObject.AddComponent<Paladin_Effect>();
            this.paladin_effect.player = player;
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
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {            
            Destroy(player.gameObject.GetComponentInChildren<Paladin_Effect>());
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
                CardTools.FormatStat(true,"Per Enemy In Range",$"{regen_amount} HP Per Second")
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
