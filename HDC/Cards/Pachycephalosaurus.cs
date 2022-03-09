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
    class Pachycephalosaurus : CustomCard
    {
        private float block_cooldown = -0.25f;
        private float health_boost = 0.25f;
        private Pachycephalosaurus_Effect pachy_effect;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("Dinosaur") };
            //block.forceToAdd = 15f;
            block.cdMultiplier = 1f + block_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //UnityEngine.Debug.Log(block.forceToAdd);
            pachy_effect = player.gameObject.GetOrAddComponent<Pachycephalosaurus_Effect>();
            pachy_effect.player = player;
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
                     
        }
        protected override string GetTitle()
        {
            return "Pachycephalosaurus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.pachycephalosaurus);
        }
        protected override GameObject GetCardArt()
        {
            
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Pachycephalosaurus");
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
                CardTools.FormatStat(true,"Block Cooldown",block_cooldown),
                CardTools.FormatStat(true,"Headbutt on Block","BIG")
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
