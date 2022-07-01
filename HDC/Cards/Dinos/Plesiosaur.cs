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
    class Plesiosaur : CustomCard
    {
        //private float submergeTime = 5f;
        private int submergeBounces = 3;
        private float healthBoost = 0.75f;
        private float damageBoost = 0.15f;
        private float gravityBoost = -0.5f;
        private Plesiosaur_Effect plesiosaur_effect;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass, Carnivore.CarnivoreClass };
            statModifiers.health = 1 + healthBoost;
            gun.bulletDamageMultiplier = 1 + damageBoost;
            gun.gravity = 1 + gravityBoost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards++;
            plesiosaur_effect = player.gameObject.GetOrAddComponent<Plesiosaur_Effect>();
            //plesiosaur_effect.outOfBoundsTime += submergeTime;
            plesiosaur_effect.floorBounces += submergeBounces;
            //characterStats.GetAdditionalData().plesioSubmergeTime += submergeTime;

            //add new plesiosaur effect here
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards--;
            //plesiosaur_effect.outOfBoundsTime -= submergeTime;
            plesiosaur_effect.floorBounces -= submergeBounces;
            if (plesiosaur_effect.floorBounces <= 0)
            {
                Destroy(player.gameObject.GetComponent<Plesiosaur_Effect>());
            }
            //characterStats.GetAdditionalData().plesioSubmergeTime -= submergeTime;
        }
        protected override string GetTitle()
        {
            return "Plesiosaur";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.plesiosaur);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Plesiosaur");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
               //CardTools.FormatStat(true,"Submerge Time",$"+{submergeTime} Seconds"),
               CardTools.FormatStat(true,"Unsinkable Bounces",submergeBounces),
               CardTools.FormatStat(true,"Health",healthBoost),
               CardTools.FormatStat(true,"Damage",damageBoost),
               CardTools.FormatStat(true,"Bullet Gravity",gravityBoost)
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
