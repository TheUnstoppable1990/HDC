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
using ModdingUtils.Extensions;
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
    class Saint : CustomCard
    {
        public static CardInfo card = null;

        public static CardCategory HolyClass = CustomCardCategories.instance.CardCategory("Holy");
        public static CardCategory[] HolyCards = new CardCategory[] { HolyClass };
        public const string SaintClassName = "Saint";

        public static CardCategory SaintClass = CustomCardCategories.instance.CardCategory("Saint");

        public static float regenPerCard = 5f;
        public static float healthPerCard = 0.05f;


        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            return card.categories.Intersect(HolyCards).Any();
        }
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>();
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Class"), CustomCardCategories.instance.CardCategory("CardManipulation") };
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

            var paleo_effect = player.gameObject.GetOrAddComponent<Saint_Effect>();
            HDC.instance.ExecuteAfterFrames(20, () =>
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, addToCardBar: true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, randomCard);
            });
        }
        public override void OnReassignCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var saint_effect = player.gameObject.GetOrAddComponent<Saint_Effect>();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Destroy(player.gameObject.GetComponentInChildren<Saint_Effect>());
        }

        protected override string GetTitle()
        {
            return "The Saint";
        }
        protected override string GetDescription()
        {
            return "Become a force for rightousness and valor with <color=#00ffff>Holy</color> Cards";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Saint");
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
                CardTools.FormatStat(true,"Health Per Card",healthPerCard),
                CardTools.FormatStat(true,"Regen Per Card",(int)regenPerCard)
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
    class Saint_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        public Saint_Regen_Effect saint_regen = null;
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            SetLivesToEffect(int.MaxValue);
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "The Saint".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                var classCards = data.currentCards.Where(card => card.categories.Contains(Saint.SaintClass)).ToList();
                var cardIndeces = Enumerable.Range(0, player.data.currentCards.Count()).Where((index) => player.data.currentCards[index].categories.Contains(Saint.SaintClass)).ToArray();
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
                    classCards.Insert(0, Saint.card);

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

        public void OnPlayerPickStart()
        {
            if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.Contains(Saint.SaintClass))
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.RemoveAll((category) => category == Saint.SaintClass);
            }
        }

        public void OnPointStart()
        {
            var holyNum = data.currentCards.Where(card => card.categories.Contains(Saint.HolyClass)).ToList().Count();

            characterDataModifier.maxHealth_mult = 1 + holyNum * Saint.healthPerCard;
            characterDataModifier.health_mult = 1 + holyNum * Saint.healthPerCard;
            
            if (saint_regen == null)
            {
                saint_regen = player.gameObject.GetOrAddComponent<Saint_Regen_Effect>();
            }
            saint_regen.healAmount = holyNum * Saint.regenPerCard;
            HDC.Log("ADDING SAINT MODIFIER");            
            ApplyModifiers();
            HDC.Log($"Player {player.playerID} has Health: {data.maxHealth}, Regen: {saint_regen.healAmount}");
            CheckIfValid();

        }

        public void OnPointEnd()
        {
            HDC.Log("REMOVING SAINT MODIFIER");
            ClearModifiers();
            HDC.Log($"Player {player.playerID} has Health: {data.maxHealth}, Regen: {characterStatModifiers.regen}");
        }

        public void OnGameStart()
        {
            Destroy(player.gameObject.GetComponentInChildren<Saint_Regen_Effect>());
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            OnPointEnd();
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStatModifiers).blacklistedCategories.Add(Saint.SaintClass);
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
            Destroy(player.gameObject.GetComponentInChildren<Saint_Regen_Effect>());
        }



    }
}

namespace HDC.MonoBehaviours
{
    class Saint_Regen_Effect : MonoBehaviour
    {
        internal Player player;
        internal CharacterData data;
        private float timePass = 0.0f;
        public float healAmount = 5.0f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?
        }
        private void OnDestroy()
        {
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?

        }
        public void Awake()
        {
            this.player = gameObject.GetComponent<Player>();
            this.data = this.player.GetComponent<CharacterData>();
        }


        private void Update()
        {          
            timePass += Time.deltaTime;            
            if (timePass > 1.0f)  //every second
            {
                this.data.healthHandler.Heal(healAmount);                
                timePass = 0.0f;
                
            }
        }
        private void ResetStuff()
        {
            timePass = 0.0f;
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
