using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using HDC.Extentions;

namespace HDC.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(HealthHandler),"Revive")]
    class HealthHandlerRevive
    {
        private static void Prefix(HealthHandler __instance, bool isFullRevive = true)
        {
            if (isFullRevive)
            {
                ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).stats.GetAdditionalData().holyLightCharge = 0f;
            }
        }
    }
}
