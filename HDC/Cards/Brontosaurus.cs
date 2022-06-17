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
    class Brontosaurus : CustomCard
    {
        private float health_boost = 1f;
        private float size_boost = 0.5f;
        private float movement_reduction = -0.25f;
        private float jump_reduction = -0.25f;
        //private float add_reload_time = 2f; //seconds
        //private float regeneration = 3f;
        private float damageOT = 5f;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("Dinosaur") };
            cardInfo.allowMultiple = false;
            statModifiers.movementSpeed = 1f + movement_reduction;
            statModifiers.secondsToTakeDamageOver = damageOT;

            statModifiers.health = 1f + health_boost;      
            //gun.reloadTimeAdd = add_reload_time;
            //statModifiers.regen = regeneration;
            statModifiers.sizeMultiplier = 1f + size_boost;
            statModifiers.jump = 1f + jump_reduction;
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
            return "Brontosaurus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.brontosaurus);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Brontosaurus");
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
                //CardTools.FormatStat(true,"Regen",$"{regeneration} hp/s"),
                CardTools.FormatStat(true,"Damage Taken Over",$"{damageOT}s"),
                CardTools.FormatStat(false,"Size",size_boost),
                CardTools.FormatStat(false,"Movement Speed",movement_reduction),
                CardTools.FormatStat(false,"Jump Height",jump_reduction)
                //CardTools.FormatStat(false,"Reload Time", $"+{add_reload_time}s")

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
