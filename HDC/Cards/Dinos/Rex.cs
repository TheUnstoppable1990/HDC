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

namespace HDC.Cards
{
    class Rex : CustomCard
    {
        private float health_boost = 1f;
        private float dmg_boost = 2f;
        private float distance = 1f;
        private float add_reload_time = 0.25f; //seconds
        private float size = 0.5f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass , Carnivore.CarnivoreClass };
            //cardInfo.allowMultiple = false;
            statModifiers.sizeMultiplier = 1f + size;
            statModifiers.health = 1f + health_boost;
            //gun.damage = 1f + dmg_boost;
            gun.bulletDamageMultiplier = 1f + dmg_boost;
            gun.destroyBulletAfter = distance;
            gun.reloadTimeAdd = add_reload_time;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards++;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards--;
        }
        protected override string GetTitle()
        {
            return "Rex";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.rex);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Rex");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Health",health_boost),
                CardTools.FormatStat(true,"Damage",dmg_boost),
                CardTools.FormatStat(false,"Size",size),
                CardTools.FormatStat(false,"Range","Reduced"),
                CardTools.FormatStat(false,"Reload Time", $"+{add_reload_time}s")
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
