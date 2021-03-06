using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using HDC.Extentions;

namespace HDC.Patches
{
    //reset cards and effects
    [Serializable]
    [HarmonyPatch(typeof(Player), "FullReset")]
    class PlayerPatchFullReset
    {
        private static void Prefix(Player __instance)
        {
            //__instance.data.stats.GetAdditionalData().holyLightCharge = 0f;
            CustomEffects.DestroyAllEffects(__instance.gameObject);
        }
    }
}
