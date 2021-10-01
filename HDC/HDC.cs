using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using Photon.Pun;
using HarmonyLib;
using HDC.Cards;
using UnboundLib.GameModes;
using Jotunn.Utils;
using HDC.MonoBehaviours;



namespace HDC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils",BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModID, ModName, ModVersion)]
    [BepInProcess("Rounds.exe")]
    public class HDC : BaseUnityPlugin
    {        
        private const string ModID = "com.theunstoppable1990.rounds.hdc";
        private const string ModName = "Hatchet Daddy's Cards (HDC)";
        public const string ModVersion = "0.0.14";
        internal static AssetBundle ArtAssets;
        void Awake()
        {
            var harmony = new Harmony(ModID);
            harmony.PatchAll();
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, ResetEffects); //Thank you Willis for this Code :)
            
        }
        IEnumerator ResetEffects(IGameModeHandler gm)
        {
            DestroyAll<Paladin_Effect>();
            DestroyAll<CelestialCountdown_Effect>();
            DestroyAll<Meditation_Effect>();
            DestroyAll<BehindYou_Effect>();
            DestroyAll<DivineBlessing_Effect>();
            yield break;
        }
        void DestroyAll<T>() where T : UnityEngine.Object
        {
            var objects = GameObject.FindObjectsOfType<T>();
            for (int i = objects.Length - 1; i >= 0; i--)
            {
                UnityEngine.Debug.Log($"Attempting to Destroy {objects[i].GetType().Name} number {i}");
                UnityEngine.Object.Destroy(objects[i]);
            }
        }

        
        void Start()
        {

            //Cards go here
            Unbound.RegisterCredits(ModName, 
                new string[] {"TheUnstoppable1990 (HatchetDaddy himself)" }, 
                new string[] { "GitHub",                                        "Have Some Bad Dinosaur Jokes" }, 
                new string[] { "https://github.com/TheUnstoppable1990/HDC",     "https://baddinopuns.tumblr.com/" });
            HDC.ArtAssets = AssetUtils.LoadAssetBundleFromResources("test_angel_card", typeof(HDC).Assembly);

            //
            CustomCard.BuildCard<DivineBlessing>();
            CustomCard.BuildCard<Meditation>();
            CustomCard.BuildCard<CelestialCountdown>();
            CustomCard.BuildCard<Paladin>();
            CustomCard.BuildCard<HolyLight>();
            CustomCard.BuildCard<BehindYou>();
            CustomCard.BuildCard<LilDefensive>();
            CustomCard.BuildCard<LilOffensive>();
            CustomCard.BuildCard<PointBlank>();

            
         
        }

        static public string FormatStat(float value)
        {

            return value.ToString();
        }
        static public CardInfoStat FormatStat(bool pos, string name, int amount, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
        {
            string formatAmount;
            formatAmount = amount.ToString();
            if (amount > 0)
            {
                formatAmount = "+" + formatAmount;
            }
            return new CardInfoStat()
            {
                positive = pos,
                stat = name,
                amount = formatAmount,
                simepleAmount = simple
            };
        }
        static public CardInfoStat FormatStat(bool pos, string name, float amount, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
        {
            string formatAmount;
            formatAmount = (amount * 100).ToString() + "%";
            if(amount > 0)
            {
                formatAmount = "+" + formatAmount;
            }
            return new CardInfoStat()
            {
                positive = pos,
                stat = name,
                amount = formatAmount,
                simepleAmount = simple
            };
        }

    }
}
