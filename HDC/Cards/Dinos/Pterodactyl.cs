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

namespace HDC.Cards
{
    class Pterodactyl : CustomCard
    {
        private float speed_boost = 0.30f;
        private float health_boost = -0.5f;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { Paleontologist.DinoClass, Carnivore.CarnivoreClass };

            statModifiers.health = 1f + health_boost;
            statModifiers.movementSpeed = 1 + speed_boost;
            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            InAirJumpEffect flight = player.gameObject.GetOrAddComponent<InAirJumpEffect>();
            flight.SetJumpMult(0.1f);
            flight.AddJumps(100);
            flight.SetCostPerJump(1);
            flight.SetContinuousTrigger(true);
            flight.SetResetOnWallGrab(true);
            flight.SetInterval(0.1f);
            gravity.gravityForce = 0.01f;

            characterStats.GetAdditionalData().numDinoCards++;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Destroy(player.gameObject.GetOrAddComponent<InAirJumpEffect>());
            characterStats.GetAdditionalData().numDinoCards--;
        }
        protected override string GetTitle()
        {
            return "Pterodactyl";
        }
        protected override string GetDescription()
        {
            return CardTools.RandomDescription(DinoPuns.pterodactyl);
        }
        protected override GameObject GetCardArt()
        {
            return HDC.ArtAssets.LoadAsset<GameObject>("C_Pterodactyl");
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
                CardTools.FormatStat(true,"(Kinda)","Flight!"),
                CardTools.FormatStat(false,"Health",health_boost)
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
