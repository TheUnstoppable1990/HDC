using System;
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



namespace HDC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModID, ModName, "0.0.1.1")]
    [BepInProcess("Rounds.exe")]
    public class HDC : BaseUnityPlugin
    {        
        private const string ModID = "com.theunstoppable1990.rounds.hdc";
        private const string ModName = "Hatchet Daddy's Cards (HDC)";
        internal static AssetBundle ArtAssets;
        void Awake()
        {
            var harmony = new Harmony(ModID);
            harmony.PatchAll();
            UnityEngine.Debug.Log("Test Test");
            UnityEngine.Debug.Log("Please let this show up");
        }

        void Start()
        {

            //Cards go here
            Unbound.RegisterCredits(ModName, new string[] { "TheUnstoppable1990 (HatchetDaddy himself)" }, new string[] { "Have Some Bad Dinosaur Jokes" }, new string[] { "https://baddinopuns.tumblr.com/" });
            HDC.ArtAssets = AssetUtils.LoadAssetBundleFromResources("test_angel_card", typeof(HDC).Assembly);


            CustomCard.BuildCard<DivineBlessing>();
            CustomCard.BuildCard<Meditation>();
            CustomCard.BuildCard<CelestialCountdownCard>();
         
        }

    }
}
