﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace HDC.Extentions
{
    [Serializable]
    public class CharacterDataAdditionalData
    {
        //public float armor;
        //public float maxArmor;
        public OutOfBoundsHandler outOfBoundsHandler;
        //public AlphaEffect alphaEffect;

        public CharacterDataAdditionalData()
        {
            //armor = 0;
            //maxArmor = 0;
            outOfBoundsHandler = null;
            //alphaEffect = null;
        }
    }

    public static class CharacterDataExtension
    {
        public static readonly ConditionalWeakTable<CharacterData, CharacterDataAdditionalData> data =
            new ConditionalWeakTable<CharacterData, CharacterDataAdditionalData>();

        public static CharacterDataAdditionalData GetAdditionalData(this CharacterData block)
        {
            return data.GetOrCreateValue(block);
        }

        public static void AddData(this CharacterData block, CharacterDataAdditionalData value)
        {
            try
            {
                data.Add(block, value);
            }
            catch (Exception) { }
        }
    }

    // CODE FROM PCE
    // get outOfBounds handler assigned to this player
    [HarmonyPatch(typeof(OutOfBoundsHandler), "Start")]
    class OutOfBoundsHandlerPatchStart
    {
        private static void Postfix(OutOfBoundsHandler __instance)
        {
            if (((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler == null)
            {
                OutOfBoundsHandler[] ooBs = UnityEngine.GameObject.FindObjectsOfType<OutOfBoundsHandler>();
                foreach (OutOfBoundsHandler ooB in ooBs)
                {
                    if (((CharacterData)Traverse.Create(ooB).Field("data").GetValue()).player.playerID == ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).player.playerID)
                    {
                        ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler = ooB;
                        return;
                    }
                }
            }
        }
    }
    /*
    public class CharacterDataAdditionalData
    {
        public OutOfBoundsHandler outOfBoundsHandler;

        public CharacterDataAdditionalData()
        {
            outOfBoundsHandler = null;
        }
    }
    public static class CharacterDataExtension
    {
        public static readonly ConditionalWeakTable<CharacterData, CharacterDataAdditionalData> data =
            new ConditionalWeakTable<CharacterData, CharacterDataAdditionalData>();

        public static CharacterDataAdditionalData GetAdditionalData(this CharacterData characterData)
        {
            return data.GetOrCreateValue(characterData);
        }

        public static void AddData(this CharacterData characterData, CharacterDataAdditionalData value)
        {
            try
            {
                data.Add(characterData, value);
            }
            catch (Exception) { }
        }
    }
    // get outOfBounds handler assigned to this player
    [HarmonyPatch(typeof(OutOfBoundsHandler), "Start")]
    class OutOfBoundsHandlerPatchStart
    {
        private static void Postfix(OutOfBoundsHandler __instance)
        {
            if (((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler == null)
            {
                OutOfBoundsHandler[] ooBs = UnityEngine.GameObject.FindObjectsOfType<OutOfBoundsHandler>();
                foreach (OutOfBoundsHandler ooB in ooBs)
                {
                    if (((CharacterData)Traverse.Create(ooB).Field("data").GetValue()).player.playerID == ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).player.playerID)
                    {
                        ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler = ooB;
                        return;
                    }
                }
            }
        }
    }

    /*
    [Serializable]
    public class CharacterDataAdditionalData
    {
        public OutOfBoundsHandler outOfBoundsHandler;

        public CharacterDataAdditionalData()
        {
            outOfBoundsHandler = null;
        }
    }

    public static class CharacterDataExtension
    {
        public static readonly ConditionalWeakTable<CharacterData, CharacterDataAdditionalData> data =
            new ConditionalWeakTable<CharacterData, CharacterDataAdditionalData>();

        public static CharacterDataAdditionalData GetAdditionalData(this CharacterData characterData)
        {
            return data.GetOrCreateValue(characterData);
        }

        public static void AddData(this CharacterData characterData, CharacterDataAdditionalData value)
        {
            try
            {
                data.Add(characterData, value);
            }
            catch (Exception) { }
        }

    }

    // get outOfBounds handler assigned to this player
    [HarmonyPatch(typeof(OutOfBoundsHandler), "Start")]
    class OutOfBoundsHandlerPatchStart
    {
        private static void Postfix(OutOfBoundsHandler __instance)
        {
            if (((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler == null)
            {
                OutOfBoundsHandler[] ooBs = UnityEngine.GameObject.FindObjectsOfType<OutOfBoundsHandler>();
                foreach (OutOfBoundsHandler ooB in ooBs)
                {
                    if (((CharacterData)Traverse.Create(ooB).Field("data").GetValue()).player.playerID == ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).player.playerID)
                    {
                        ((CharacterData)Traverse.Create(__instance).Field("data").GetValue()).GetAdditionalData().outOfBoundsHandler = ooB;
                        return;
                    }
                }
            }
        }
    }
    */
}
