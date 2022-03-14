using UnityEngine;
//using UnityEngine.ParticleSystemModule;
using Photon.Pun;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnboundLib.Utils;
using System.Reflection;
using System.Linq;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Networking;
using System.Collections;
using HarmonyLib;
using HDC.Extentions;
using UnboundLib.Cards;
using Sonigon;
using Sonigon.Internal;


namespace HDC.MonoBehaviours
{
    class HatchetAssets
    {
        private static GameObject _hatchet = null;
        internal static GameObject hatchet
        {
            get
            {
                if(_hatchet != null)
                {
                    return _hatchet;
                }
                else
                {
                    _hatchet = new GameObject("Hatchet_Swing", typeof(HatchetSwingEffect), typeof(PhotonView));
                    UnityEngine.GameObject.DontDestroyOnLoad(_hatchet);

                    return _hatchet;
                }
            }
            set { }
        }
    }

    public class HatchetSpawner : MonoBehaviour
    {
        private static bool Initialized = false;
        
        void Awake()
        {
            if (!Initialized)
            {
                PhotonNetwork.PrefabPool.RegisterPrefab(HatchetAssets.hatchet.name, HatchetAssets.hatchet);//string "Hatchet_Swing", GameObject hatchet
            }
        }

        void Start()
        {
            HDC.Log("HatchetSpawner Start");
            if (!Initialized)
            {
                Initialized = true;
                return;
            }
            HDC.Log("Checking projectile is mine?");
            if(!PhotonNetwork.OfflineMode && !this.gameObject.transform.parent.GetComponent<ProjectileHit>().ownPlayer.data.view.IsMine)
            {
                HDC.Log("Not our projectile");
                return;
            }

            HDC.Log("Instantiating object");
            var name = HatchetAssets.hatchet.name;
            HDC.Log($"Name Retrieved: {name}");
            PhotonView photonView = gameObject.transform.parent.GetComponent<PhotonView>();
            if (!photonView)
            {
                HDC.Log("PhotonView is null, aborting start");
                return;
            }

            HDC.Log("PhotonView: " + photonView);
            int viewID = photonView.ViewID;
            HDC.Log("ViewID retrieved: " + viewID);
            PhotonNetwork.Instantiate(
                name,
                transform.position,
                transform.rotation,
                0,
                new object[] {viewID}
                );
        }
    }
    
    [RequireComponent(typeof(PhotonView))]
    public class HatchetSwingEffect : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private Player player;
        private Gun gun;
        private ProjectileHit projectile;

        public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            GameObject parent = PhotonView.Find((int)instantiationData[0]).gameObject;
            gameObject.transform.SetParent(parent.transform);
            HDC.Log("photon instantiate for hatchet swing");
            foreach (Transform child in gameObject.transform.parent)
            {
                HDC.Log("Children of parent: " + child.gameObject);
            }
            projectile = gameObject.transform.parent.GetComponent<ProjectileHit>();
            foreach(Transform child in projectile.transform)
            {
                HDC.Log("children of projectile: " + child.gameObject);
            }
            player = projectile.ownPlayer;
            HDC.Log(player + "is player");
            gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
            HDC.Log(gun + "is gun");
        }
        void Awake()
        {
            HDC.Log("Waking up");
            
        }
        void Start()
        {
            HDC.Log("Starting Hatchet Swing");
            var visual = GenerateVisual();
            var range = CalculateRange(gun);
            HDC.Log("Triggering visuals");
            TriggerVisual(visual, range);


            HDC.Log("Getting targets");
            var targetsInRange = GetInRangeTargets(player.transform.position, range);
            HDC.Log("Beginning explosion logic");
            foreach (Collider2D target in targetsInRange)
            {
                HDC.Log("Checking for rigidbody on collider " + target + " from attachedRigidbody: " + target.attachedRigidbody + " from component in parent: " + target.GetComponentInParent<Rigidbody2D>());
                var rigidbody = target.GetComponentInParent<Rigidbody2D>();
                if (rigidbody)
                {
                    HDC.Log("rigidbody found");
                    DoPushRigidbody(player.transform.position, rigidbody, (gun.damage / 2)); //took out extention multiplier
                }
                else
                {
                    HDC.Log("Checking for player on collider " + target + ": " + target.GetComponentInParent<Player>());
                    var otherPlayer = target.GetComponentInParent<Player>();
                    if (otherPlayer)
                    {
                        HDC.Log("player found");
                        DoPushCharData(player.transform.position, otherPlayer, (gun.damage / 2)); //took out e
                    }
                }
                HDC.Log("Checking for damageable on collider " + target);
                var damageable = target.transform.gameObject.GetComponent<Damagable>();
                if (damageable)
                {
                    HDC.Log("damageable found");
                    DoDamage(target.GetComponent<Damagable>());
                }
            }

            //this is where the bullets are getting removed forever
            projectile.projectileColor = Color.black;
            projectile.bulletCanDealDeamage = false;
            projectile.sendCollisions = false;
            projectile.transform.position = new Vector3(-1000f, -1000f, -1000f);
        }
        
        private void DoPushRigidbody(Vector2 origin, Rigidbody2D rigidbody, float force) // force function might be unecessary
        {
            HDC.Log("Doing push");
            HDC.Log("Adding force " + force + " in direction " + (rigidbody.position - origin).normalized + "For a net value of " + ((rigidbody.position - origin).normalized * force * rigidbody.mass));
            rigidbody.AddForce((rigidbody.position - origin).normalized * force * rigidbody.mass * 0.75f);
        }
        private void DoPushCharData(Vector2 origin, Player otherPlayer, float force)
        {
            HDC.Log("Doing push for player");
            HDC.Log(" adding force " + force + " for total vector " + ((Vector2)otherPlayer.transform.position - origin).normalized * force * 2);
            var healthHandler = otherPlayer.GetComponentInChildren<HealthHandler>();
            healthHandler.CallTakeForce(((Vector2)otherPlayer.transform.position - origin).normalized * force * 2);
            HDC.Log("Post force");
        }
        private void DoDamage(Damagable damageable)
        {
            HDC.Log("Doing damage");
            var totalDamage = Vector2.up * 55 * gun.damage * gun.bulletDamageMultiplier / 1.5f;
            HDC.Log("Gun damage: " + gun.damage + " bulletDamageMultiplier " + gun.bulletDamageMultiplier + " Total damage: " + totalDamage + " transform? " + player.transform);
            damageable.CallTakeDamage(Vector2.up * 55 * gun.damage * gun.bulletDamageMultiplier / 1.5f, player.transform.position,null,player);
        }
















        private GameObject GenerateVisual()//adjust for hatchet swing effect when made
        {
            GameObject _shockblastVisual;
            List<CardInfo> activecards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList();
            List<CardInfo> inactivecards = (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            List<CardInfo> allcards = activecards.Concat(inactivecards).ToList();
            GameObject E_Overpower = allcards.Where(card => card.cardName.ToLower() == "overpower").First().GetComponent<CharacterStatModifiers>().AddObjectToPlayer.GetComponent<SpawnObjects>().objectToSpawn[0];
            _shockblastVisual = UnityEngine.GameObject.Instantiate(E_Overpower, new Vector3(0, 100000f, 0f), Quaternion.identity);
            _shockblastVisual.name = "E_Discombobulate";
            DontDestroyOnLoad(_shockblastVisual);
         //   foreach (ParticleSystem parts in _shockblastVisual.GetComponentsInChildren<ParticleSystem>())
           // {
             //   parts.startColor = Color.cyan;
               // parts.startLifetime = parts.startLifetime / 2;
            //}
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().colorOverTime.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.cyan, 0f) };
            UnityEngine.GameObject.Destroy(_shockblastVisual.transform.GetChild(2).gameObject);
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().offsetMultiplier = 0f;
            _shockblastVisual.transform.GetChild(1).GetComponent<LineEffect>().playOnAwake = true;
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<FollowPlayer>());
            _shockblastVisual.GetComponent<DelayEvent>().time = 0f;
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<SoundImplementation.SoundUnityEventPlayer>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<Explosion>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<Explosion_Overpower>());
            UnityEngine.GameObject.Destroy(_shockblastVisual.GetComponent<RemoveAfterSeconds>());
            return _shockblastVisual;
        }

        private float CalculateRange(Gun gun) //This might not be necessary long term
        {
            HDC.Log("Gun is " + gun);
            var range = gun.GetAdditionalData().hatchetReach + ((float)Math.Sqrt(gun.projectileSpeed) * 1.2f);
            HDC.Log("gun.projectileSpeed: " + gun.projectileSpeed);
            HDC.Log("Range: " + range);
            return range;
        }
        private void TriggerVisual(GameObject visual, float range)
        {
            HDC.Log("Setting scale");
            visual.transform.localScale = new Vector3(1f, 1f, 1f);
            HDC.Log("Adding removeAfterSeconds");
            visual.AddComponent<RemoveAfterSeconds>().seconds = 5f;
            HDC.Log("Initializing line effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("inited", false);
            typeof(LineEffect).InvokeMember("Init",
                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                null, visual.transform.GetChild(1).GetComponent<LineEffect>(), new object[] { });
            HDC.Log("Adjusting line effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().radius = (range);
            visual.transform.GetChild(1).GetComponent<LineEffect>().SetFieldValue("startWidth", 0.5f);
            visual.transform.position = player.transform.position;
            HDC.Log("Playing effect");
            visual.transform.GetChild(1).GetComponent<LineEffect>().Play();
        }



        private ISet<Collider2D> GetInRangeTargets(Vector2 origin, float range)
        {
            ISet<Collider2D> targets = new HashSet<Collider2D>();
            var colliders = Physics2D.OverlapCircleAll(origin, range); //maybe can rework this to front view only
            var playerCollider = player.GetComponent<Collider2D>();
            HDC.Log("Player collider? " + playerCollider + "At position " + player.transform.position);
            foreach (Collider2D collider in colliders)
            {
                HDC.Log("Looking at collider (" + collider + ") for gameobject " + collider.gameObject + " at position " + collider.transform.position);
                if (!collider.Equals(player.GetComponent<Collider2D>()))
                {
                    HDC.Log("Checking if collider (" + collider + ") for gameobject " + collider.gameObject + " is visible");
                    //Eliminates encountered colliders for anything without a rigidbody except players, which apparently don't have one. Go figure.
                    var list = Physics2D.RaycastAll(origin, (((Vector2)collider.transform.position) - origin).normalized, range).Select(item => item.collider).Where(item => !item.Equals(playerCollider) && (item.attachedRigidbody || (item.GetComponentInParent<Player>() && item.GetComponentInParent<Player>().playerID != player.playerID))).ToList();
                    list.ForEach(item => HDC.Log("raycast item: " + item.gameObject + "is the collider we're looking at? " + (item.Equals(collider))));
                    if (list.Count > 0 && list[0].Equals(collider))
                    {
                        HDC.Log("Item matched, adding to targets: " + collider.transform.gameObject);
                        targets.Add(collider);
                    }
                }
            }
            return targets;
        }        

    }
    internal class A_Hatchet: MonoBehaviour
    {
        
        
        internal static IEnumerator RemoveAllHatchets(IGameModeHandler gm)
        {

            foreach(Player player in PlayerManager.instance.players)
            {
                try
                {
                    A_Hatchet.MakeHatchet(player.playerID, false);
                }
                catch { }
            }
            yield break;

        }
        internal const float SwitchDelay = 0.10f; //might not need this
        public const float Volume = 1f;

        private float SlashTimer = 0f;
        public bool IsOut { get; private set; } = false; //might not need this either
        private Player Player;//i dont like that this is capitalized
        public float SwitchTimer { get; private set; } = 0f;
        //SoundEvent HatchetPullOutSound; //probs dont need this either but cool
        void Start()
        {
            this.IsOut = false;
            this.SlashTimer = 0f;
            this.SwitchTimer = 0f;
            this.Player = this.GetComponentInParent<Player>();

        }
        void Update()
        {
            if (this.Player is null || !this.Player.data.view.IsMine || this.Player.data.dead)
            {
                return;
            }
            this.SlashTimer -= TimeHandler.deltaTime;
            this.SwitchTimer -= TimeHandler.deltaTime;

            /*
            if (this.SwitchTimer <= 0f && this.Player.data.playerActions.ItemWasPressed(2))
            {
                this.SwitchTimer = this.IsOut ? 0f : SwitchDelay;
                this.IsOut = !this.IsOut;
                if (this.IsOut)
                {
                    // play sound locally
                    SoundManager.Instance.Play(this.ClawPullOutSound, this.Player.transform, new SoundParameterBase[] { new SoundParameterIntensity(Optionshandler.vol_Master * Optionshandler.vol_Sfx * Volume) });
                }
                NetworkingManager.RPC(typeof(A_Claw), nameof(RPCA_Switch_To_Claw), this.Player.playerID, this.IsOut);
            }
            */
            //this is all switching stuff, dunno if i need this
        }
        [UnboundRPC]
        internal static void RPCA_Switch_To_Hatchet(int slashingPlayerID, bool hatchet)
        {
            MakeHatchet(slashingPlayerID, hatchet);            
        }
        internal void MoveToHide(Transform transform, bool hide)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, hide ? -10000f : 0f);
        }
        internal static void MakeHatchet(int playerID, bool hatchet)
        {
            
            Player player = PlayerManager.instance.players.FirstOrDefault(p => p.playerID == playerID);
            if(player == null)
            {
                return;
            }
            Gun gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
            GameObject springObj = gun.transform.GetChild(1).gameObject;
            RightLeftMirrorSpring spring = springObj.transform.GetChild(2).GetComponent<RightLeftMirrorSpring>();

            GameObject Hatchet = springObj.transform.Find("HDC_Hatchet(Clone)")?.gameObject;
            if(Hatchet is null)
            {
                Hatchet = GameObject.Instantiate(HDC.ArtAssets.LoadAsset<GameObject>("A_Hatchet"), springObj.transform);


                //Come back to this
                //you're not done here

            }
        }
    }
}
