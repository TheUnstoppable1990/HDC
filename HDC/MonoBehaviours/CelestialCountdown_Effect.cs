using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.Networking;
using System.Collections;
using System.ComponentModel;
using Sonigon;
using Sonigon.Internal;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using Photon.Pun;
using Photon.Realtime;

namespace HDC.MonoBehaviours
{
    public class CelestialCountdown_Effect : MonoBehaviour
    {
        public SoundEvent soundCelestialChargeLoop;
        
        //private bool soundChargeIsPlaying;
        
        private float soundCounterLast;
       
        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);
        
        [Range(0f, 1f)]
        public float counter;

        public float timeToFill = 7f;

        public float timeToEmpty = 3f;

        public float duration;

        public float hpMultiplier = 2f;

        public ProceduralImage outerRing; // this should all be set when card implemented

        public ProceduralImage fill;
                
        public Transform rotator;
                
        public Transform still;
                
        private CharacterData data;
                
        public GameObject[] celestialObjects; //might change name

        private float remainingDuration;

        private bool isCelestialForm; //might change name

        private float startCounter;

        public Gun gun;
        public GunAmmo gunAmmo;
        public CharacterStatModifiers characterStats;
        public Gravity gravity;

        public float defaultRLTime;
        private float reloadModifier = 0.001f;
        private float aSpeedModifier = 0.25f;

        public Player player;
        private SetTeamColor[] teamColors;


        //Work around for simulated position issue
        private Vector2 lastPosition = Vector2.zero;
        //private int count = 0;
        //private float distChange = 0.00f;

        private void Start()
        {           
            this.soundCounterLast = this.counter;
            this.data = base.GetComponentInParent<CharacterData>();
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?
            base.GetComponentInParent<ChildRPC>().childRPCs.Add("Celestial", new Action(this.RPCA_Activate));
            this.lastPosition = this.data.playerVel.position;
            this.teamColors = this.player.transform.root.GetComponentsInChildren <SetTeamColor>();
        }
        private void OnDestroy()
        {
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?
            base.GetComponentInParent<ChildRPC>().childRPCs.Remove("Celestial");
        }
        private void OnDisable()
        {
            //this.SoundStop();
        }       
        
        private void ResetStuff()
        {
            //this.SoundStop();
            this.remainingDuration = 0f;
            this.counter = 0f;
            if (this.isCelestialForm)
            {               
                this.alterStats(false);                
                this.isCelestialForm = false;
            }
        }
        private void RPCA_Activate()
        {
            UnityEngine.Debug.Log("RPCA_Activate");
            this.remainingDuration = this.duration;
        }
        private void alterStats(bool enable)
        {
            if (enable)
            {
                this.data.maxHealth *= this.hpMultiplier;
                this.data.health *= this.hpMultiplier;
                this.gunAmmo.reloadTimeMultiplier = this.reloadModifier;
                this.gun.attackSpeed *= this.aSpeedModifier;
                this.gravity.gravityForce /= 2f;
                this.characterStats.movementSpeed *= 2f;
                this.characterStats.sizeMultiplier *= 2f;
            }
            else
            {
                this.data.maxHealth /= this.hpMultiplier;
                this.data.health /= this.hpMultiplier;
                this.gunAmmo.reloadTimeMultiplier = this.defaultRLTime;
                this.gun.attackSpeed /= this.aSpeedModifier;

                this.gravity.gravityForce *= 2f;
                this.characterStats.movementSpeed /= 2f;
                this.characterStats.sizeMultiplier /= 2f;
            }
        }
        private void Update()
        {
           
            //UnityEngine.Debug.Log(this.startCounter);


            //if (this.soundCounterLast < this.counter)
            //{
            //    this.SoundPlay();
            //}
            //else
            //{
            //    this.SoundStop();
            //}
            //this.soundCounterLast = this.counter;
            //this.soundParameterIntensity.intensity = this.counter;
            //this.outerRing.fillAmount = this.counter;
            //this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));
            if (this.data.playerVel.position.y > 100)
            {
                this.startCounter = 1f;
                return;
            }
            this.startCounter -= TimeHandler.deltaTime;
            if (this.startCounter > 0f)
            {
               // this.remainingDuration = this.duration;
                return;
            }
            //
            if (this.remainingDuration > 0f)//Handles Duration countdown when Celestial Form is to be active
            {
                
                if (!this.isCelestialForm)//Activates Celstial Form
                {
                    //Buffed stats Go Here
                    this.alterStats(true);
                    //Work around for this? 
                    //this.data.stats.ConfigureMassAndSize();
                    this.isCelestialForm = true;
                    UnityEngine.Debug.Log("Celestial Form Active");
                }
                this.remainingDuration -= TimeHandler.deltaTime;//reduces duration here
                this.counter = this.remainingDuration / this.duration;
                return; //Breaks out before further conditionals
            }
            
            if (this.isCelestialForm) //Occurs when duration is zero or less
            {
                //Remove Buffs here
                this.alterStats(false);
                this.ConfigureMassAndSize();
                this.isCelestialForm = false;
                UnityEngine.Debug.Log("Celestial Form Disabled");                
            }
            
            if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down)
            {
              
                this.counter += TimeHandler.deltaTime / this.timeToFill; //Fills the circle when not moving?
               
            }
            else
            {
           
                this.counter -= TimeHandler.deltaTime / this.timeToEmpty; //empties circle

            }
            this.counter = Mathf.Clamp(this.counter, -0.1f / this.timeToFill, 1f);
            if (this.counter >= 1f && this.data.view.IsMine)
            {
                this.remainingDuration = this.duration;
                base.GetComponentInParent<ChildRPC>().CallFunction("Celestial");
            }
            if (this.counter <= 0f)
            {
                //this.rotator.gameObject.SetActive(false);
                //this.still.gameObject.SetActive(false);
                return;
            }
            //this.rotator.gameObject.SetActive(true);
            //this.still.gameObject.SetActive(true);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
        private void ConfigureMassAndSize()
        {
            base.transform.localScale = Vector3.one * 1.2f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.2f) * 1f;//size multiplier? not sure if static or not
        }
        
        
        
    }
}
