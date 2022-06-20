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
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ClassesManagerReborn.Util;
using System.Collections.ObjectModel;
using UnboundLib.Utils;
using System.Reflection;
using HDC.Cards;
using System.Collections;
using ModdingUtils.GameModes;
using ModdingUtils.Extensions;
using HDC.Class;



namespace HDC.Cards
{
    class Prayer : CustomCard
    {

        public static CardInfo card = null;

        private float mobility = -0.50f;

        private Prayer_Effect prayer_effect;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = SaintClass.name;
        }
        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return card.categories.Intersect(Saint.HolyCards).Any();
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };

            statModifiers.movementSpeed = 1 + mobility;
            statModifiers.jump = 1 + mobility;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CardInfo randomCard = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, this.condition);
            if (randomCard == null)
            {
                // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                randomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, player, null, null, null, null, null, null, null, this.condition);
            }
            HDC.instance.ExecuteAfterFrames(20, () =>
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, addToCardBar: true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, randomCard);
            });

            //adding the prayer effect here
            prayer_effect = player.gameObject.AddComponent<Prayer_Effect>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //remove effect here
            HDC.Log($"Removing Prayer");
            Destroy(player.gameObject.GetComponentInChildren<Prayer_Effect>());
        }
        protected override string GetTitle()
        {
            return "Humble Prayer";
        }
        protected override string GetDescription()
        {
            return "Pray for a random <color=#00ffff>Holy</color> Card. Your mobility is hindered while you pray.";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Prayer");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(false,"Movement Speed",mobility),
                CardTools.FormatStat(false,"Jump Height",mobility)
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

namespace HDC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class Prayer_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        private int turns = 3;


        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            SetLivesToEffect(int.MaxValue);
        }
        public void OnPlayerPickStart()
        {

        }

        public void OnPointStart()
        {

        }

        public void OnPointEnd()
        {
            turns--;
            HDC.Log($"Prayer Turns left: {turns}");

            if (turns <= 0)
            {
                CardTools.RemoveFirstCardByName(player, "Humble Prayer");             
                Destroy(this);
            }
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            HDC.Log("Prayer OnOnDestroy Attempt");
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }



    }
}


