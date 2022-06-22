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
    class Compsognathus : CustomCard
    {
        //private float sizeDecrease = -0.25f;
        private float healthDecrease = -0.5f;
        private int ammoIncrease = 3;
        private int bulletIncrease = 2;
        private float damageDecrease = -0.40f;

        //hidden pack stats to make em more chasey and junk
        private int bounces = 2;
        private float grav = 0.5f;
        private float bullSpeed = 0.75f;

        private List<ObjectsToSpawn> gunObjects;
        private ObjectsToSpawn compyObj;
        

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass, Carnivore.CarnivoreClass };
            statModifiers.sizeMultiplier = 1 + healthDecrease;
            statModifiers.health = 1 + healthDecrease;
            //gun.damage = 1 + damageDecrease;
            gun.bulletDamageMultiplier = 1 + damageDecrease;
            gun.ammo = ammoIncrease;
            gun.numberOfProjectiles = bulletIncrease;
            gun.spread = 0.1f;


            //pack stats applied
            gun.projectileColor = new Color(0, 1, 0);
            gun.reflects = bounces;
            gun.gravity = grav;
            gun.projectileSpeed = bullSpeed;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gunObjects = gun.objectsToSpawn.ToList();
            compyObj = new ObjectsToSpawn();
            compyObj.AddToProjectile = new GameObject("A_Compy", new Type[]
            {
                typeof(Compy_Effect)
            });            
            gunObjects.Add(compyObj);           
            gun.objectsToSpawn = gunObjects.ToArray();

            characterStats.GetAdditionalData().numDinoCards++;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //need to add a removal method here            
            gunObjects = gun.objectsToSpawn.ToList();
            gunObjects.Remove(compyObj);
            gun.objectsToSpawn = gunObjects.ToArray();

            characterStats.GetAdditionalData().numDinoCards--;
        }
        protected override string GetTitle()
        {
            return "Compsognathus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.compsognathus);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Compsognathus");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
               CardTools.FormatStat(true,"Bullets",$"+{bulletIncrease} Pack"),
               //CardTools.FormatStat(true,"Bounces",bounces),
               CardTools.FormatStat(true,"Ammo",ammoIncrease),
               //CardTools.FormatStat(true,"Size",sizeDecrease),
               CardTools.FormatStat(false,"Health & Size",healthDecrease),
               CardTools.FormatStat(false,"Damage",damageDecrease)
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
