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
using System.ComponentModel;

namespace HDC.Cards
{
    class TheDebugCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var block = gameObject.AddComponent<Block>();
            block.InvokeMethod("ResetStats");
            block.cdMultiplier = 0.1f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(
                block.BlockAction,
                new Action<BlockTrigger.BlockTriggerType>(
                    this.GetDoBlockAction(player, gun, gunAmmo, data, health, gravity, block, characterStats)                    
                )
            );
        }
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                UnityEngine.Debug.Log($"Player ID: {player.playerID} Team ID: {player.teamID}");
                UnityEngine.Debug.Log($"Player Position: {data.playerVel.position.x}, {data.playerVel.position.y}");
                UnityEngine.Debug.Log(TimeHandler.deltaTime);


            };
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
        }
        protected override string GetTitle()
        {
            return "The Debug Card";
        }
        protected override string GetDescription()
        {
            return "Spits out a bunch of info when you block";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {

                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.NatureBrown;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
