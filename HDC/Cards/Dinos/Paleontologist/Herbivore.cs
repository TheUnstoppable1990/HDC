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
        public static CardInfo card = null;

        public static CardCategory HerbivoreClass = CustomCardCategories.instance.CardCategory("Herbivore");   //defining the Herbivore class for herbivore dinos
        public static CardCategory[] herbCards = new CardCategory[] { HerbivoreClass };

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
            if (herbivore_healing_effect == null)
            {
                herbivore_healing_effect = player.gameObject.GetOrAddComponent<Herbivore_Healing_Effect>();
            }
            herbivore_healing_effect.healAmount = dinos * Herbivore.healingPerCard;
            HDC.Log($"Healing effect applied for {dinos * Herbivore.healingPerCard}");
        }

        public void OnPointEnd()
        {
            
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

