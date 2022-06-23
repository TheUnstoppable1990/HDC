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
//using ModdingUtils.Extensions;
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
    class RendingClaws : CustomCard
    {
        public static CardInfo card = null; 
        public static float damagePerCard = 0.20f;
        private float damage_boost = 0.25f;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = PaleontologistClass.name;
        }        
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.damage = 1 + damage_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var rending_effect = player.gameObject.GetOrAddComponent<RendingClaws_Effect>();
            rending_effect.numOfClaws++;            
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var rending_effect = player.gameObject.GetComponent<RendingClaws_Effect>();
            rending_effect.numOfClaws--;
            if (rending_effect.numOfClaws <= 0)
            {
                Destroy(player.gameObject.GetComponentInChildren<RendingClaws_Effect>());
            }
        }
        protected override string GetTitle()
        {
            return "Rending Claws";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Claws");
        }
        protected override string GetDescription()
        {
            return "Razor sharp, rending claws";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Damage",damage_boost),
                CardTools.FormatStat(true,"Damage per <color=#00ff00>Dino</color>",damagePerCard)
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
    class RendingClaws_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        public int numOfClaws = 0;
        private float multiplier;
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
            multiplier = 1 + (RendingClaws.damagePerCard * dinos * numOfClaws);
            gun.damage *= multiplier;            
        }

        public void OnPointEnd()
        {
            gun.damage /= multiplier;
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
