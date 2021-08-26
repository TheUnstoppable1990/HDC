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
namespace HDC.Cards
{
    class CelestialCountdown : CustomCard
    {
        private Player thisPlayer;
        private CelestialCountdown_Effect countdown;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //UnityEngine.Debug.Log(data);
            this.thisPlayer = player;
            this.countdown = player.gameObject.AddComponent<CelestialCountdown_Effect>();
            countdown.player = player;
            countdown.duration = 10f;
            countdown.hpMultiplier = 10f;
            countdown.celestialObjects = new GameObject[] { };
            countdown.gun = gun;
            countdown.gunAmmo = gunAmmo;
            countdown.defaultRLTime = gun.reloadTime;
            countdown.characterStats = characterStats;
            countdown.gravity = gravity;
            //Regeneration regeneration = player.gameObject.AddComponent<Regeneration>();
            //data.maxHealth *= 1.25f;
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
            Destroy(this.countdown);
            
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
            try
            {
                return HDC.ArtAssets.LoadAsset<GameObject>("C_AngelCard");
            }
            catch
            {
                UnityEngine.Debug.Log("Something went wrong with card art");
                return null;
            }
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
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
