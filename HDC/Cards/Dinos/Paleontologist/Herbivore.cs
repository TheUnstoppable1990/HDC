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
    class Herbivore : CustomCard
    {
        public static CardCategory[] dinoCards = new CardCategory[] { Paleontologist.DinoClass };
        public static CardInfo card = null;

        public static float healingPerCard = 5f;
        private float healthBonus = 0.5f;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = PaleontologistClass.name;
        }        
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            statModifiers.health = 1 + healthBonus;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var herbivore_effect = player.gameObject.AddComponent<Herbivore_Effect>();
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {            
            Destroy(player.gameObject.GetComponentInChildren<Herbivore_Effect>());
        }
        protected override string GetTitle()
        {
            return "Herbivore";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Herbivore");
        }
        protected override string GetDescription()
        {
            return "Learn from the plant-eating <color=#00ff00ff>Dinosaurs</color>.";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Max Health",healthBonus),
                CardTools.FormatStat(true,"Regen per <color=#00ff00>Dino</color> Card",(int)healingPerCard)
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
    class Herbivore_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        

        public Herbivore_Healing_Effect herbivore_healing_effect = null;

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
                if (player.data.currentCards[i].cardName.ToLower() == "Herbivore".ToLower())
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
            //additional_health = Herbivore.healthPerCard * dinos;
            //player.data.maxHealth *= additional_health;
            //player.data.health = player.data.maxHealth;
            if (herbivore_healing_effect == null)
            {
                herbivore_healing_effect = player.gameObject.GetOrAddComponent<Herbivore_Healing_Effect>();
            }
            herbivore_healing_effect.healAmount = dinos * Herbivore.healingPerCard;
            //ApplyModifiers();
            //HDC.Log($"Player {player.playerID} has max health: {player.data.stats.health}");
            HDC.Log($"Healing effect applied for {dinos * Herbivore.healingPerCard}");
            //CheckIfValid();

        }

        public void OnPointEnd()
        {
            //HDC.Log("REMOVING DINO MODIFIER");
            //ClearModifiers();
            //player.data.maxHealth *= additional_health;
            //player.data.health = player.data.maxHealth;
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
            Destroy(player.gameObject.GetComponentInChildren<Herbivore_Healing_Effect>());
        }
    }
}

namespace HDC.MonoBehaviours
{
    class Herbivore_Healing_Effect : MonoBehaviour
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

