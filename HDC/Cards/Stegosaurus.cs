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
using HDC.RoundsEffects;

namespace HDC.Cards
{
    class Stegosaurus : CustomCard
    {
        public static float plate_reduction = 0.5f;
        private int plateNum = 1;
        private float speed_boost = -0.1f; //Speed Reduction in truth          
        private int extra_blocks = 2;
        private CharacterStatModifiers stats;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("Dinosaur") };

            block.additionalBlocks = extra_blocks;
            statModifiers.movementSpeed = 1 + speed_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            stats = characterStats;
            characterStats.GetAdditionalData().stegoPlates += plateNum ;
        }
        public override void OnRemoveCard()
        {            
            stats.GetAdditionalData().stegoPlates -= plateNum;
        }
        protected override string GetTitle()
        {
            return "Stegosaurus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.stegosaurus);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Stegosaurus");            
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Extra Blocks",extra_blocks),                
                CardTools.FormatStat(true,"Plates",plateNum),
                CardTools.FormatStat(true,"Dmg Red / Plate",plate_reduction),
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
