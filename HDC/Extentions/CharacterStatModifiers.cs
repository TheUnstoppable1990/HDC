using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

//Totally stole this method from PCE

namespace HDC.Extentions
{


    [Serializable] 
    public class CharacterStatModifiersAdditionalData //making a place for additional stats
    {
        public int stegoPlates;
        public int armorPlates;
        public float holyLightCharge;
        public int panicAuras;
        public float piercePercent;
        public int numDinoCards;
        public int diloCards;
        //public float plesioSubmergeTime;
        public int trikes;
         
        public CharacterStatModifiersAdditionalData()
        {
            armorPlates = 0;
            stegoPlates = 0;
            holyLightCharge = 0f;
            panicAuras = 0;
            piercePercent = 0f;
            numDinoCards = 0;
            diloCards = 0;
            trikes = 0;
            //plesioSubmergeTime = 0f;
        }

    }
    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData> data =
            new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData>();

        public static CharacterStatModifiersAdditionalData GetAdditionalData(this CharacterStatModifiers characterstats)
        {
            return data.GetOrCreateValue(characterstats);
        }

        public static void AddData(this CharacterStatModifiers characterstats, CharacterStatModifiersAdditionalData value)
        {
            try
            {
                data.Add(characterstats, value);
            }
            catch (Exception) { }
        }

    }
    // reset additional CharacterStatModifiers when ResetStats is called
    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetAdditionalData().stegoPlates = 0;
            __instance.GetAdditionalData().armorPlates = 0;
            __instance.GetAdditionalData().holyLightCharge = 0f;
            __instance.GetAdditionalData().panicAuras = 0;
            __instance.GetAdditionalData().piercePercent = 0f;
            __instance.GetAdditionalData().numDinoCards = 0;
            __instance.GetAdditionalData().diloCards = 0;
            __instance.GetAdditionalData().trikes = 0;
            //__instance.GetAdditionalData().plesioSubmergeTime = 0f;
        }
    }
}
