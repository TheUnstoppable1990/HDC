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
        private float panic_block_cd = -0.6f;
        private CharacterStatModifiers stats;

        public Parasaurolophus_Effect panic_effect;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("Dinosaur") };
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            this.stats = characterStats;
            this.stats.GetAdditionalData().panicAuras++;
            int pa = this.stats.GetAdditionalData().panicAuras;
            panic_effect = player.gameObject.GetOrAddComponent<Parasaurolophus_Effect>();

            float adjustedBCD = (float)Math.Pow(1f + panic_block_cd, pa) - 1f; //gets the multiplier value
            HDC.Log(adjustedBCD.ToString());
            panic_effect.panic_speed += this.panic_speed/pa;
            panic_effect.panic_regen += this.panic_regen/pa;
            panic_effect.panic_block_cd = adjustedBCD; //returns it to the original format

            panic_effect.AdjustSize(pa);
        }
        
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {         
            int pa = this.stats.GetAdditionalData().panicAuras;
            panic_effect.panic_speed -= this.panic_speed / pa; //adjust before changing number
            panic_effect.panic_regen -= this.panic_regen / pa;
            panic_effect.panic_block_cd -= this.panic_block_cd / pa;
            this.stats.GetAdditionalData().panicAuras--;
            pa = this.stats.GetAdditionalData().panicAuras;
            panic_effect.AdjustSize(pa);            
            if (pa < 1)
            {
                //Destroy(panic_effect);
                Destroy(player.gameObject.GetComponentInChildren<Parasaurolophus_Effect>());
            }
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
