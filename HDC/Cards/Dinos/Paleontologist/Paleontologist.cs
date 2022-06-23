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
using RarityLib.Utils;

namespace HDC.Cards
{
    public class Paleontologist : CustomCard
    {
        public static float multiplier = 0.1f;
        public static CardInfo card = null;

        public static CardCategory DinoClass = CustomCardCategories.instance.CardCategory("Dinosaur");  //this defines the category of dinosaur for the dino cards
        public static CardCategory[] dinoCards = new CardCategory[] { DinoClass };                      //this is the grounp of dino cards
        public static string PaleontologistClassName = "Dino";                                           //this is the classname for paleontologist


        public static CardCategory PaleontologistClass = CustomCardCategories.instance.CardCategory("Paleontologist");  //this defines the Paleontologist class, different from dino class
        

        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
            return card.categories.Intersect(dinoCards).Any();
        }

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>();
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Class") , CustomCardCategories.instance.CardCategory("CardManipulation") };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;          
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

            var paleo_effect = player.gameObject.GetOrAddComponent<Paleontologist_Effect>();
            HDC.instance.ExecuteAfterFrames(20, () =>
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, addToCardBar: true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, randomCard);
            });

        }
        public override void OnReassignCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var paleo_effect = player.gameObject.GetOrAddComponent<Paleontologist_Effect>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Destroy(player.gameObject.GetComponentInChildren<Paleontologist_Effect>());            
        }

        protected override string GetTitle()
        {
            return "Paleontologist";
        }
        protected override string GetDescription()
        {
            return "Get bonuses for every dinosaur card";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Paleontologist");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            //return CardInfo.Rarity.Rare;
            return RarityUtils.GetRarity("Legendary");
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Health/Dino",multiplier),
                CardTools.FormatStat(true,"Damage/Dino",multiplier),
                CardTools.FormatStat(true,"Speed/Dino",multiplier),
                CardTools.FormatStat(true,"Dino Card","+1")
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
namespace HDC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class Paleontologist_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler, IRoundStartHookHandler, IRoundEndHookHandler,
        IBattleStartHookHandler
    {
        private float multiplier;
        private float DinoMult()
        {
            return 1f + Paleontologist.multiplier * data.stats.GetAdditionalData().numDinoCards;
        }
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            SetLivesToEffect(int.MaxValue);
        }
        
        public void OnPlayerPickStart()
        {
            if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.Contains(Paleontologist.PaleontologistClass))
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.RemoveAll((category) => category == Paleontologist.PaleontologistClass);
            }
        }
        public void OnRoundStart()
        {
            HDC.Log("Running the Round Start Code");
        }
        public void OnPointStart()
        {
            HDC.Log("Running the Point Start Code");
            int dinos = data.currentCards.Where(card => card.categories.Contains(Paleontologist.DinoClass)).ToList().Count();
            HDC.Log($"Player {player.playerID} has {dinos} dinos");

            multiplier = 1f + Paleontologist.multiplier * dinos;

            data.maxHealth *= multiplier;
            data.health = data.maxHealth;
            gun.damage *= multiplier;
            data.stats.movementSpeed *= multiplier;
            HDC.instance.ExecuteAfterFrames(20, () =>
            {
                HDC.Log($"Player {player.playerID} has Health: {data.maxHealth}, Damage: {gun.damage}, Speed: {characterStatModifiers.movementSpeed}");
            });
        }

        public void OnPointEnd()
        {
            HDC.Log("Running the Point End Code");
           
            data.maxHealth /= multiplier;
            data.health = data.maxHealth;
            gun.damage /= multiplier;
            data.stats.movementSpeed /= multiplier;
            HDC.instance.ExecuteAfterFrames(20, () =>
            {
                HDC.Log($"Player {player.playerID} has Health: {data.maxHealth}, Damage: {gun.damage}, Speed: {characterStatModifiers.movementSpeed}");
            });
        }
        public void OnRoundEnd()
        {
            HDC.Log("Running the Round End Code");
        }
        public void OnBattleStart()
        {
            HDC.Log("Running the Battle Start Code");            
        }
        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            OnPointEnd();
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.Add(Paleontologist.PaleontologistClass);
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }



    }
}
