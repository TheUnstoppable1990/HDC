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

namespace HDC.Cards
{
    class DivineBlessing : CustomCard
    {
        private Block thisBlock;
        private Action<BlockTrigger.BlockTriggerType> healAction;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var block = gameObject.AddComponent<Block>();
            block.InvokeMethod("ResetStats");
            block.cdMultiplier = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            this.thisBlock = block;
            healAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(player, block, data));
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, healAction);

        }
        private float healAmount = 10f;
        private float healRatio = 0.25f;
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block, CharacterData data)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                healAmount = data.maxHealth * healRatio;
                if (data.health <= (data.maxHealth - healAmount))
                {
                    data.health += healAmount;
                }
                else if (data.health < data.maxHealth && data.health > (data.maxHealth - healAmount))
                {
                    data.health = data.maxHealth;
                }
                UnityEngine.Debug.Log($"{data.health}/{data.maxHealth}");
            };
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
            thisBlock.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(thisBlock.BlockAction, healAction);
        }
        protected override string GetTitle()
        {
            return "Divine Blessing";
        }
        protected override string GetDescription()
        {
            return "Heal a small amount of hp everytime you block";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block Cooldown",
                    amount = "25%",
                    simepleAmount = CardInfoStat.SimpleAmount.lower
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = "25%",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                }
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
