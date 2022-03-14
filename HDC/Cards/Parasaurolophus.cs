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

namespace HDC.Cards
{
    class Parasaurolophus : CustomCard
    {
        private float panic_speed = 0.5f;
        private float panic_regen = 0.25f;
        private float panic_block_cd = -0.75f;

        private Parasaurolophus_Effect panic_effect;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("Dinosaur") };
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            panic_effect = player.gameObject.GetOrAddComponent<Parasaurolophus_Effect>();
            panic_effect.panic_speed = this.panic_speed;
            panic_effect.panic_regen = this.panic_regen;
            panic_effect.panic_block_cd = this.panic_block_cd;
        }
        public override void OnRemoveCard()
        {
            Destroy(panic_effect);
        }
        protected override string GetTitle()
        {
            return "Parasaurolophus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.parasaurolophus);
        }
        protected override GameObject GetCardArt()
        {            
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Parasaurolophus");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
               CardTools.FormatStat(true,"","When Panicking:"),
               CardTools.FormatStat(true,"Speed",panic_speed),
               CardTools.FormatStat(true,"Regeneration",$"{panic_regen*100}%/s"),
               CardTools.FormatStat(true,"Block Cooldown",panic_block_cd)
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
