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
using HDC.RoundsEffects;

namespace HDC.Cards
{
    public class Triceratops : CustomCard
    {
        public static float horns_duration = 1f; // 1 second
        public static float damage_const = 100f;

        private float health_boost = 0.5f;
        //private float speed_boost = 0.25f;
        private float block_cooldown = -0.30f;
        private float add_reload_time = 0.25f; //seconds

        private Triceratops_Effect trike_effect;

        
        
        

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass, Herbivore.HerbivoreClass };

            //statModifiers.movementSpeed = 1 + speed_boost;
            statModifiers.health = 1f + health_boost;                        
            gun.reloadTimeAdd = add_reload_time;
            block.cdMultiplier = 1f + block_cooldown;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards++;
            characterStats.GetAdditionalData().trikes++;
            trike_effect = player.gameObject.GetOrAddComponent<Triceratops_Effect>();
            trike_effect.block = block;
            //add custom damaging Effect here
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAdditionalData().numDinoCards--;
            characterStats.GetAdditionalData().trikes--;
            if(characterStats.GetAdditionalData().trikes <= 0)
            {
                Destroy(player.gameObject.GetComponent<Triceratops_Effect>());
            }
        }
        protected override string GetTitle()
        {
            return "Triceratops";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.triceratops);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Triceratops");            
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Block Cooldown",block_cooldown),
                CardTools.FormatStat(true,"Health",health_boost),
                //CardTools.FormatStat(true,"Movement Speed",speed_boost),         
                CardTools.FormatStat(true,"On Block","Horns Attack"),
                CardTools.FormatStat(false,"Reload Time", $"+{add_reload_time}s")                
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
