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
        private float block_cooldown = 0.25f;
        private float health_boost = 0.25f;
        private Pachycephalosaurus_Effect pachy_effect;
        private float knockbackMult = 100f;
        private float range = 10f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass, Herbivore.HerbivoreClass};
            block.cdMultiplier = 1f + block_cooldown;
            statModifiers.health = 1f + health_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            pachy_effect = player.gameObject.GetOrAddComponent<Pachycephalosaurus_Effect>();            
            pachy_effect.player = player;
            pachy_effect.knockbackMult += knockbackMult;
            pachy_effect.range += range;
            //HDC.Log(pachy_effect.GetStats());

            characterStats.GetAdditionalData().numDinoCards++;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {            
            Destroy(player.gameObject.GetComponentInChildren<Pachycephalosaurus_Effect>());
            characterStats.GetAdditionalData().numDinoCards--;
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
                CardTools.FormatStat(true,"Headbutt on Block","BIG"),
                CardTools.FormatStat(false,"Block Cooldown",block_cooldown)
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
