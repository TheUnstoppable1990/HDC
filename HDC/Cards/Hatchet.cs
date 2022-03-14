using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.Networking;
using System.Collections;
using HDC.MonoBehaviours;
using UnityEngine.UI.ProceduralImage;
using HDC.Extentions;
namespace HDC.Cards
{
    class Hatchet : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.transform.localScale = new Vector3(0.01f, 0.01f); //making the gun disappear
            cardInfo.allowMultiple = false;
            gun.GetAdditionalData().hatchetReach += 2.5f;
            ObjectsToSpawn hatchetSwing = new ObjectsToSpawn() { };
            hatchetSwing.AddToProjectile = new GameObject("HatchetSpawner", typeof(HatchetSpawner));
            gun.objectsToSpawn = new ObjectsToSpawn[] { hatchetSwing };

            GameObject hatchetArt = HDC.ArtAssets.LoadAsset<GameObject>("A_Hatchet");
            
        }
        public override void OnRemoveCard()
        {
            //base.OnRemoveCard();
        }
        protected override GameObject GetCardArt()
        {
            //throw new NotImplementedException();
            return null;
        }
        protected override string GetTitle()
        {
            return "Hatchet";
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[] { };
        }
        protected override string GetDescription()
        {
            return "Who needs guns anyways";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;            
        }
        public override string GetModName()
        {
            return "HDC";
        }
        
    }
}
