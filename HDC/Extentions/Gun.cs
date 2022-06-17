using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnboundLib;

namespace HDC.Extentions
{
    [Serializable]
    public class GunAdditionalData //may be borrowing this idea from PCE
    {        
        public float hatchetReach;
        public bool disabled = false;
        public GunAdditionalData()
        {
            hatchetReach = 0f;
            disabled = false;
        }
    }

    public static class GunExtension
    {
        public static readonly ConditionalWeakTable<Gun, GunAdditionalData> data =
            new ConditionalWeakTable<Gun, GunAdditionalData>();

        public static GunAdditionalData GetAdditionalData(this Gun gun)
        {
            return data.GetOrCreateValue(gun);
        }

        public static void AddData(this Gun gun, GunAdditionalData value)
        {
            try
            {
                data.Add(gun, value);
            }
            catch (Exception) { }
        }
    }
   
    [HarmonyPatch(typeof(Gun),"ResetStats")]
    class GunPatchResetStats
    {
        private static void Prefix(Gun __instance)
        {
            __instance.GetAdditionalData().hatchetReach = 0f;
            __instance.GetAdditionalData().disabled = false;
        }
    }
}
