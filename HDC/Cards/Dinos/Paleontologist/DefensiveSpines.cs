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
using ModdingUtils.RoundsEffects;
using HDC.RoundsEffects;

namespace HDC.Cards
{
    class DefensiveSpines : CustomCard
    {
        public static CardInfo card = null; 
        public static float dmgPerSpine = 0.10f;
        private float health_boost = 0.25f;

        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = PaleontologistClass.name;
        }        
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            statModifiers.health = 1 + health_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var spines_effect = player.gameObject.GetOrAddComponent<DefensiveSpines_Effect>();
            spines_effect.numOfSpines++;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var spines_effect = player.gameObject.GetOrAddComponent<DefensiveSpines_Effect>();
            spines_effect.numOfSpines--;
            if (spines_effect.numOfSpines <= 0)
            {
                Destroy(player.gameObject.GetComponentInChildren<DefensiveSpines_Effect>());
            }
        }
        protected override string GetTitle()
        {
            return "Defensive Spines";
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Ankylosaurus");
        }
        protected override string GetDescription()
        {
            return "Damage inducing defensive spines";
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Health",health_boost),
                CardTools.FormatStat(true,"Spines Per <color=#00ff00>Dino</color>",1),
                CardTools.FormatStat(true,"Thorns Per Spine",dmgPerSpine)
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
    class DefensiveSpines_Effect : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        public int numOfSpines = 0;
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
            float spine_percent = numOfSpines * DefensiveSpines.dmgPerSpine;
            var spine_hit_effect = player.gameObject.GetOrAddComponent<SpinesOnHit_Effect>();
            spine_hit_effect.damage_percent = spine_percent;
            HDC.Log($"Thorns Percent: {spine_hit_effect.damage_percent * 100}%a");
        }

        public void OnPointEnd()
        {

        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(player.gameObject.GetComponentInChildren<SpinesOnHit_Effect>());
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
namespace HDC.RoundsEffects
{
    class SpinesOnHit_Effect : WasHitEffect
    {
        public float damage_percent = 0f;
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            Player attacking_player = this.gameObject.GetComponent<Player>().data.lastSourceOfDamage;
            if (!selfDamage && attacking_player != null)
            {
                Vector2 horns_damage = new Vector2(-damage_percent * damage.x, -damage_percent * damage.y);
                Vector2 enemy_pos = attacking_player.data.playerVel.position;
                attacking_player.data.healthHandler.DoDamage(horns_damage, enemy_pos, Color.blue);
            }
        }
    }
}
