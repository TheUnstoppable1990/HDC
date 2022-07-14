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
    [BepInDependency("pykess.rounds.plugins.legraycasterspatch", BepInDependency.DependencyFlags.HardDependency)] // fixes physics for small players
    [BepInPlugin(ModID, ModName, ModVersion)]
    [BepInProcess("Rounds.exe")]
    public class HDC : BaseUnityPlugin
    {                
        private const string ModID = "com.theunstoppable1990.rounds.hdc";
        private const string ModName = "Hatchet Daddy's Cards (HDC)";
        public const string ModVersion = "1.2.7";
        internal static AssetBundle ArtAssets;
        //private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources(, typeof(HDC).Assembly);

        public static HDC instance { get; private set; }

        //Remember to change this before release

        private static bool debugging = false;

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
            //DestroyAll<DivineBlessing_Effect>();
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

            //CustomCard.BuildCard<Saint>(cardInfo => { Saint.card = cardInfo; });

            //CustomCard.BuildCard<Prayer>(cardInfo => { Prayer.card = cardInfo; });

            //Signature Cards
            CustomCard.BuildCard<BehindYou>();
            CustomCard.BuildCard<LilDefensive>();
            CustomCard.BuildCard<LilOffensive>();
            CustomCard.BuildCard<PointBlank>();
                              

            //Dino Cards
                //Carnivores
            CustomCard.BuildCard<Rex>();
            CustomCard.BuildCard<Raptor>();
            CustomCard.BuildCard<Pterodactyl>();
            CustomCard.BuildCard<Compsognathus>();
            CustomCard.BuildCard<Dilophosaurus>();
            CustomCard.BuildCard<Plesiosaur>();
                //Herbivores
            CustomCard.BuildCard<Stegosaurus>();
            CustomCard.BuildCard<Brontosaurus>();
            CustomCard.BuildCard<Pachycephalosaurus>();
            CustomCard.BuildCard<Ankylosaurus>();
            CustomCard.BuildCard<Parasaurolophus>();
            CustomCard.BuildCard<Triceratops>();



            CustomCard.BuildCard<Paleontologist>(cardInfo => { Paleontologist.card = cardInfo; });
            CustomCard.BuildCard<DigSite>(cardInfo => { DigSite.card = cardInfo; });
            CustomCard.BuildCard<Carnivore>(cardInfo => { Carnivore.card = cardInfo; });
            CustomCard.BuildCard<PiercingTeeth>(cardInfo => { PiercingTeeth.card = cardInfo; });
            CustomCard.BuildCard<RendingClaws>(cardInfo => { RendingClaws.card = cardInfo; });
            CustomCard.BuildCard<Herbivore>(cardInfo => { Herbivore.card = cardInfo; });
            CustomCard.BuildCard<ArmorPlates>(cardInfo => { ArmorPlates.card = cardInfo; });
            CustomCard.BuildCard<DefensiveSpines>(cardInfo => { DefensiveSpines.card = cardInfo; });

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

        public static void PropertyDump(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
            {
                Log("Object is null, no properties found.");
            }
            else
            {
                var type = obj.GetType();
                foreach (var prop in type.GetProperties())
                {
                    var val = prop.GetValue(obj, new object[] { });
                    var valStr = val == null ? "" : val.ToString();
                    props.Add(prop.Name, valStr);
                }
                Log($"Object: {obj.GetType()} has the following properties:");
                foreach (var property in props)
                {
                    Log($"\t{property.Key}: {property.Value}");
                }
            }
        }
    }
}
