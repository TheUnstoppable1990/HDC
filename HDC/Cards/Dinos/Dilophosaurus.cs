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
    class Dilophosaurus : CustomCard
    {

        private float damage_boost = 0.25f;
        private float reload_time = 0.25f;
        public static ObjectsToSpawn toxicObjects = null; //maybe make static?
        //public static GameObject diloObject = null;
        private static ObjectsToSpawn diloObjects = null;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass , Carnivore.CarnivoreClass};
            gun.bulletDamageMultiplier = 1 + damage_boost;
            gun.reloadTime = 1 + reload_time;

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards++;
            characterStats.GetAdditionalData().diloCards++;
            if(diloObjects == null)
            {
                diloObjects = new ObjectsToSpawn();                
                toxicObjects = ((GameObject)Resources.Load("0 cards/Toxic cloud")).GetComponent<Gun>().objectsToSpawn[0];
                                
                GameObject diloATP = toxicObjects.AddToProjectile is null ? null : Instantiate(toxicObjects.AddToProjectile);
                diloATP?.SetActive(false);
                
                GameObject diloEffect = toxicObjects.effect is null ? null : Instantiate(toxicObjects.effect);
                diloEffect?.SetActive(false);

                GameObject diloObject = Instantiate(toxicObjects.effect.GetComponent<SpawnObjects>().objectToSpawn[0]);
                diloObject.SetActive(false);

                diloObject.GetComponent<ParticleSystem>().startColor = new Color(0.16f, 0, 0.29f);

                HDC.Log("Dilo Spawn Objects: " + diloEffect.GetComponent<SpawnObjects>());
                diloEffect.GetComponent<SpawnObjects>().objectToSpawn[0] = diloObject;
                diloObjects.AddToProjectile = diloATP;
                diloObjects.effect = diloEffect;


                diloObjects.direction = toxicObjects.direction;
                diloObjects.normalOffset = toxicObjects.normalOffset;
                diloObjects.numberOfSpawns = toxicObjects.numberOfSpawns;
                diloObjects.removeScriptsFromProjectileObject = toxicObjects.removeScriptsFromProjectileObject;
                diloObjects.scaleFromDamage = toxicObjects.scaleFromDamage;
                diloObjects.scaleStackM = toxicObjects.scaleStackM;
                diloObjects.scaleStacks = toxicObjects.scaleStacks;
                diloObjects.spawnAsChild = toxicObjects.spawnAsChild;
                diloObjects.spawnOn = toxicObjects.spawnOn;
                //diloObjects.stacks = toxicObjects.stacks;
                diloObjects.stickToAllTargets = toxicObjects.stickToAllTargets;
                diloObjects.stickToBigTargets = toxicObjects.stickToBigTargets;
                diloObjects.zeroZ = toxicObjects.zeroZ;


                //Color Change Code but it changes the base thing and we dont want that probably...
                //var e_toxicCloud = diloObjects.effect.GetComponent<SpawnObjects>().objectToSpawn[0];

                //diloObject = GameObject.Instantiate(e_toxicCloud);
                HDC.Log("Dilo Object Active? "+diloObjects.ToString());
            }
            else
            {
                HDC.Log("Toxic Objects already exists");                
            }
            diloObjects.stacks = characterStats.GetAdditionalData().diloCards - 1;

            if (characterStats.GetAdditionalData().diloCards <= 1)
            {
                List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();

                //list.Add(toxicObjects);
                list.Add(diloObjects);
                gun.objectsToSpawn = list.ToArray();
            }

            HDC.Log("Gun Objects to spawn" + gun.objectsToSpawn[0].ToString());
            //only ever adds 1 instance of the cloud and just stacks damage

            //may wanna come back and eventually limit stack size but for now lets let it be fun
            
        }        
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards--;            
            characterStats.GetAdditionalData().diloCards--;
            if (characterStats.GetAdditionalData().diloCards > 0)
            {
                diloObjects.stacks = characterStats.GetAdditionalData().diloCards - 1;
            }
            else
            {
                List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
                if (list.Contains(diloObjects))    //It should contain it, but just to avoid errors
                {
                    list.Remove(diloObjects);
                    gun.objectsToSpawn = list.ToArray();
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No Toxic Objects Found even though Dilophosaur was removed");
                }
            }
        }
        protected override string GetTitle()
        {
            return "Dilophosaurus";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.dilophosaurus);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Dilophosaurus");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
               CardTools.FormatStat(true,"Damage",damage_boost),
               CardTools.FormatStat(true,"Bullets","Toxic Spit"),
               CardTools.FormatStat(false,"Reload Time",reload_time)
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
