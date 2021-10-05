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

namespace HDC.Cards
{
    class Raptor : CustomCard
    {
        private float speed_boost = 1f;
        private float reload_time = -0.5f;
        private float attack_speed = -0.5f;
        private float bullet_speed = 0.5f;
        private float damage_boost = -0.5f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            statModifiers.movementSpeed = 1 + speed_boost;
            gun.reloadTime = 1 + reload_time;
            gun.attackSpeed = 1 + attack_speed;
            gun.projectileSpeed = 1 + bullet_speed;
            gun.damage = 1 + damage_boost;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
            
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
                     
        }
        protected override string GetTitle()
        {
            return "Raptor";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.raptor);
        }
        protected override GameObject GetCardArt()
        {
            return null;            
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                CardTools.FormatStat(true,"Movement Speed",speed_boost),
                CardTools.FormatStat(true,"Reload Time",reload_time),
                CardTools.FormatStat(true,"Attack Speed",-attack_speed),
                CardTools.FormatStat(true,"Bullet Speed",bullet_speed),
                CardTools.FormatStat(false,"Damage",damage_boost)                
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return "HDC";
        }
    }
}
