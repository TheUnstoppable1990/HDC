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

//need to add blockforce patch as dependency

namespace HDC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils",BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)] //for paleontologist
    [BepInDependency("root.classes.manager.reborn",BepInDependency.DependencyFlags.HardDependency)]//for new class cards
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]//for new class cards
    [BepInPlugin(ModID, ModName, ModVersion)]
    [BepInProcess("Rounds.exe")]
    public class HDC : BaseUnityPlugin
    {                
        private const string ModID = "com.theunstoppable1990.rounds.hdc";
        private const string ModName = "Hatchet Daddy's Cards (HDC)";
        public const string ModVersion = "1.1.3";
        internal static AssetBundle ArtAssets;
        //private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources(, typeof(HDC).Assembly);

        public static HDC instance { get; private set; }

        //Remember to change this before release

        private static bool debugging = true;

        public static float auraConst = 1.375f;// stil trying to tweak this

        void Awake()
        {
            var harmony = new Harmony(ModID);
            harmony.PatchAll();
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, ResetEffects); //Thank you Willis for this Code :)
            
        }
        IEnumerator ResetEffects(IGameModeHandler gm)
        {
            DestroyAll<Paladin_Effect>();
            DestroyAll<CelestialCountdown_Effect_2>();
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
            instance = this;

            //Cards go here
            Unbound.RegisterCredits(ModName, 
                new string[] {"TheUnstoppable1990 (HatchetDaddy himself)" }, 
                new string[] { "GitHub",                                        "Have Some Bad Dinosaur Jokes" }, 
                new string[] { "https://github.com/TheUnstoppable1990/HDC",     "https://baddinopuns.tumblr.com/" });
            //ART PART
            HDC.ArtAssets = AssetUtils.LoadAssetBundleFromResources("hdc_card_asset_bundle", typeof(HDC).Assembly);

            //Angel Cards
            CustomCard.BuildCard<DivineBlessing>();
            CustomCard.BuildCard<Meditation>();
            CustomCard.BuildCard<CelestialCountdown>();
            CustomCard.BuildCard<Paladin>();
            CustomCard.BuildCard<HolyLight>();

            //Signature Cards
            CustomCard.BuildCard<BehindYou>();
            CustomCard.BuildCard<LilDefensive>();
            CustomCard.BuildCard<LilOffensive>();
            CustomCard.BuildCard<PointBlank>();

                   

            //Dino Cards
            CustomCard.BuildCard<Rex>();
            CustomCard.BuildCard<Raptor>();
            CustomCard.BuildCard<Triceratops>();
            CustomCard.BuildCard<Pterodactyl>();
            CustomCard.BuildCard<Stegosaurus>();
            CustomCard.BuildCard<Brontosaurus>();
            CustomCard.BuildCard<Pachycephalosaurus>();
            CustomCard.BuildCard<Ankylosaurus>();
            CustomCard.BuildCard<Parasaurolophus>();

            CustomCard.BuildCard<Paleontologist>(cardInfo => { Paleontologist.card = cardInfo; });

            CustomCard.BuildCard<DigSite>(cardInfo => {DigSite.card = cardInfo; });
         
        }
        internal static void Log(string message)
        {            
            if (debugging)
            {
                UnityEngine.Debug.Log(message);
            }
        }
        internal static void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        static public string FormatStat(float value)
        {

            return value.ToString();
        }

    }
}
