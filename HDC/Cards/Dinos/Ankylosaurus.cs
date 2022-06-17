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
using HDC.Extentions;
using ModdingUtils.MonoBehaviours;
using HDC.RoundsEffects;

namespace HDC.Cards
{
    class Ankylosaurus : CustomCard
    {
        private float knockback_boost = 2.0f;
        private float horns_damage = 0.5f;
        private float health_boost = 0.5f;
        private float speed_boost = -0.25f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass };
            statModifiers.health = 1 + health_boost;
            statModifiers.movementSpeed = 1 + speed_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.knockback += knockback_boost * Math.Abs(gun.knockback);
            AnkylosaurusEffect ankyEffect = player.gameObject.GetOrAddComponent<AnkylosaurusEffect>();
            ankyEffect.damage_percent += horns_damage;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (player.gameObject.GetComponent<AnkylosaurusEffect>() != null)
            {
                var ankyEffect = player.gameObject.GetComponent<AnkylosaurusEffect>();
                ankyEffect.damage_percent -= horns_damage;
                if(ankyEffect.damage_percent <= 0)
                {
                    Destroy(player.gameObject.GetComponentInChildren<AnkylosaurusEffect>());
                }
            }                      
        }
        protected override string GetTitle()
        {
            return "Ankylosaurus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.ankylosaurus);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Ankylosaurus");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
               CardTools.FormatStat(true,"Knockback",knockback_boost),
               CardTools.FormatStat(true,"Thorns Damage",horns_damage),
               CardTools.FormatStat(true,"Health",health_boost),
               CardTools.FormatStat(false,"Movement Speed",speed_boost)
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
