using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using HDC.Extentions;
using HDC.MonoBehaviours;
using HDC.Cards;
using UnboundLib;

namespace HDC.Patches
{    
    [Serializable]
    [HarmonyPatch(typeof(HealthHandler),"Heal")]
    class HealthHandlerHeal
    {
        private static void Prefix(Player ___player, ref float healAmount)
        {
            var health = ___player.data.health;
            var maxHealth = ___player.data.maxHealth;
            if (healAmount > 0f && health < maxHealth) 
            {
                //HDC.Log($"{healAmount} Healing Called for Player {___player.playerID}");
                var holyLight = ___player.GetComponent<HolyLight_Effect>();                
                if (holyLight)
                {
                    ___player.data.stats.GetAdditionalData().holyLightCharge += healAmount;
                    //HDC.Log($"Holy Light Charge: {___player.data.stats.GetAdditionalData().holyLightCharge}");
                }
            }
            
        }
    }
}
