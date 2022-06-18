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
using ModdingUtils.Extensions;
using CardChoiceSpawnUniqueCardPatch;
using System.Collections.ObjectModel;
using System.Reflection;
using UnboundLib.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ClassesManagerReborn.Util;
using HDC.Class;
using ModdingUtils.MonoBehaviours;
using ModdingUtils.GameModes;
using HDC.Cards;

namespace HDC.Cards
{
    class DigSite : CustomCard
    {
        public static CardCategory[] dinoCards = new CardCategory[] { Paleontologist.DinoClass };
        public static CardInfo card = null;

        private Fossilized_Effect fossil_effect;

        private float multiplier = -0.25f;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = PaleontologistClass.name;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };

            statModifiers.health = 1 + multiplier;
            statModifiers.movementSpeed = 1 + multiplier;
            gun.damage = 1 + multiplier;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CardInfo randomCard = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, this.condition);            
            if(randomCard == null)
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

            //adding the fossilization effect
            fossil_effect = player.gameObject.AddComponent<Fossilized_Effect>();
            
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            HDC.Log($"Removing Fossilized");
            Destroy(player.gameObject.GetComponentInChildren<Fossilized_Effect>());
            
        }
        protected override string GetTitle()
        {
            return "Dig Site";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Fossilized");
        }
        protected override string GetDescription()
        {
            return "Get a random <color=#00ff00ff>Dinosaur</color> card, but be <color=#964B00>Fossilized</color> for 3 turns";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(false,"Health",multiplier),
                CardTools.FormatStat(false,"Movement Speed",multiplier),
                CardTools.FormatStat(false,"Damage",multiplier)
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

        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return card.categories.Intersect(DigSite.dinoCards).Any();
        }

    }
}

namespace HDC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class Fossilized_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
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
            HDC.Log($"Turns left: {turns}");

            if (turns <= 0)
            {
                var fossilCard = player.data.currentCards.FirstOrDefault(c => c.cardName == "Dig Site");
                ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, fossilCard, ModdingUtils.Utils.Cards.SelectionType.Oldest, true);

                Destroy(this);

            }
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            HDC.Log("Fossilized OnOnDestroy Attempt");
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }



    }
}
