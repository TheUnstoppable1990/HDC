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
using System.Collections;
using RarityLib.Utils;

namespace HDC.Cards
{
    class Carnivore : CustomCard
    {
        public static CardCategory[] dinoCards = new CardCategory[] { Paleontologist.DinoClass };
        public static CardInfo card = null;

        public static float damagePerCard = 0.25f;
        private float lifesteal = 0.5f;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = PaleontologistClass.name;
        }        
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var carnivore_effect = player.gameObject.AddComponent<Carnivore_Effect>();

            if(characterStats.lifeSteal < 1)
            {
                characterStats.lifeSteal += lifesteal;
            }
            else
            {
                characterStats.lifeSteal *= (1 + lifesteal);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {            
            Destroy(player.gameObject.GetComponentInChildren<Carnivore_Effect>());
        }
        protected override string GetTitle()
        {
            return "Carnivore";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Carnivore");
        }
        protected override string GetDescription()
        {
            return "Learn from the meat-eating <color=#00ff00ff>Dinosaurs</color>.";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Lifesteal",lifesteal),
                CardTools.FormatStat(true,"Damage per <color=#00ff00>Dino</color> Card",damagePerCard)
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
    class Carnivore_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        //private float additional_lifesteal;
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            SetLivesToEffect(int.MaxValue);
        }
        /*
        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Carnivore".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                var classCards = data.currentCards.Where(card => card.categories.Contains(Paleontologist.PaleontologistClass)).ToList();
                var cardIndeces = Enumerable.Range(0, player.data.currentCards.Count()).Where((index) => player.data.currentCards[index].categories.Contains(Paleontologist.PaleontologistClass)).ToArray();
                if (classCards.Count() > 0)
                {
                    CardInfo[] replacePool = null;
                    if (classCards.Where(card => card.rarity == CardInfo.Rarity.Common).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Common).ToArray();
                    }
                    else if (classCards.Where(card => card.rarity == CardInfo.Rarity.Uncommon).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Uncommon).ToArray();
                    }
                    else if (classCards.Where(card => card.rarity == CardInfo.Rarity.Rare).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Rare).ToArray();
                    }
                    var replaced = replacePool[UnityEngine.Random.Range(0, replacePool.Length)];
                    classCards.Remove(replaced);
                    if (classCards.Count() > 1)
                    {
                        classCards.Shuffle();
                    }
                    classCards.Insert(0, Paleontologist.card);

                    StartCoroutine(ReplaceCards(player, cardIndeces, classCards.ToArray()));
                }
                else
                {
                    UnityEngine.GameObject.Destroy(this);
                }
            }
        }

        private IEnumerator ReplaceCards(Player player, int[] indeces, CardInfo[] cards)
        {
            yield return ModdingUtils.Utils.Cards.instance.ReplaceCards(player, indeces, cards, null, true);

            yield break;
        }
        */
        public void OnPlayerPickStart()
        {
            if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.Contains(Paleontologist.PaleontologistClass))
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.RemoveAll((category) => category == Paleontologist.PaleontologistClass);
            }
        }

        public void OnPointStart()
        {
            var dinos = data.currentCards.Where(card => card.categories.Contains(Paleontologist.DinoClass)).ToList().Count();
            //additional_lifesteal = Carnivore.lifestealPerCard * dinos;
            //player.data.stats.lifeSteal += additional_lifesteal;
            gunStatModifier.damage_mult = 1 + (Carnivore.damagePerCard * dinos);
            ApplyModifiers();
            HDC.Log($"Player {player.playerID} has Damage: {gun.damage}");
            //CheckIfValid();

        }

        public void OnPointEnd()
        {
            HDC.Log("REMOVING DINO MODIFIER");
            ClearModifiers();
            HDC.Log($"Player {player.playerID} has Lifesteal: {gun.damage}");
            //player.data.stats.lifeSteal -= additional_lifesteal;
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
