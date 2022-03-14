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
namespace HDC.Cards
{
    class CelestialCountdown : CustomCard
    {        
        private CelestialCountdown_Effect_2 cel_countdown;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            //Will's code
            var abyssalCard = CardChoice.instance.cards.First(c => c.name.Equals("AbyssalCountdown"));
            var statMods = abyssalCard.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var abyssalObj = statMods.AddObjectToPlayer;

            var celObj = Instantiate(abyssalObj, player.transform);
            celObj.name = "A_CelestialCircle";
            celObj.transform.localPosition = Vector3.zero;

            var abyssal = celObj.GetComponent<AbyssalCountdown>();

            cel_countdown = celObj.AddComponent<CelestialCountdown_Effect_2>();
            cel_countdown.soundUpgradeChargeLoop = abyssal.soundAbyssalChargeLoop;
            cel_countdown.counter = 0;
            cel_countdown.timeToFill = 7f;
            cel_countdown.timeToEmpty = 10f;
            cel_countdown.outerRing = abyssal.outerRing;
            cel_countdown.fill = abyssal.fill;
            cel_countdown.rotator = abyssal.rotator;
            cel_countdown.still = abyssal.still;
            cel_countdown.player = player;
            cel_countdown.duration = 5f;
            cel_countdown.hpMultiplier = 10f;
            cel_countdown.gun = gun;
            cel_countdown.gunAmmo = gunAmmo;
            cel_countdown.defaultRLTime = gun.reloadTime;
            cel_countdown.characterStats = characterStats;
            cel_countdown.gravity = gravity;

            HDC.instance.ExecuteAfterFrames(5, () =>
            {
                try
                {
                    UnityEngine.GameObject.Destroy(abyssal);

                    var COs = celObj.GetComponentsInChildren<Transform>().Where(child => child.parent == celObj.transform).Select(child => child.gameObject).ToArray();

                    foreach (var CO in COs)
                    {
                        if (CO.transform.gameObject != celObj.transform.Find("Canvas").gameObject)
                        {
                            UnityEngine.GameObject.Destroy(CO);
                        }
                    }
                }
                catch (Exception e)
                {
                    HDC.Log("First Catch");
                    HDC.LogException(e);
                }
                try
                {
                    cel_countdown.outerRing.color = new Color32(255, 255, 200, 255);
                    cel_countdown.fill.color = new Color32(0, 255, 255, 10);
                    cel_countdown.rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = cel_countdown.outerRing.color;
                    cel_countdown.still.gameObject.GetComponentInChildren<ProceduralImage>().color = cel_countdown.outerRing.color;
                    celObj.transform.Find("Canvas/Size/BackRing").GetComponent<ProceduralImage>().color = new Color32(33, 255, 255, 29);
                }
                catch (Exception e)
                {
                    HDC.Log("Last Catch");
                    HDC.LogException(e);
                }
            });

            //HDC.instance.DebugLog($"[{HDC.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
            HDC.Log("I added the thing!");

        }
        public override void OnRemoveCard()
        {
            Destroy(this.cel_countdown);            
        }
        protected override string GetTitle()
        {
            return "Celestial Countdown";
        }
        protected override string GetDescription()
        {
            return "Stand still to unleash divine power";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Celestial_Countdown");           
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {  
                
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
