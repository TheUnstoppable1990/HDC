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
            if(toxicObjects == null)
            {
                toxicObjects = ((GameObject)Resources.Load("0 cards/Toxic cloud")).GetComponent<Gun>().objectsToSpawn[0];
                HDC.Log("Toxic Cloud Effect Name is: " + toxicObjects.effect.name);             
                //gonna try and pinpoint the toxic cloud effect via unity and change the color
            }
            else
            {
                HDC.Log("Toxic Objects already exists");                
            }
            toxicObjects.stacks = characterStats.GetAdditionalData().diloCards - 1;

            if (characterStats.GetAdditionalData().diloCards <= 1)
            {
                List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
                list.Add(toxicObjects);
                gun.objectsToSpawn = list.ToArray();
            }
            //only ever adds 1 instance of the cloud and just stacks damage

            //may wanna come back and eventually limit stack size but for now lets let it be fun
            
        }        
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards--;            
            characterStats.GetAdditionalData().diloCards--;
            if (characterStats.GetAdditionalData().diloCards > 0)
            {
                toxicObjects.stacks = characterStats.GetAdditionalData().diloCards - 1;
            }
            else
            {
                List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList();
                if (list.Contains(toxicObjects))    //It should contain it, but just to avoid errors
                {
                    list.Remove(toxicObjects);
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
