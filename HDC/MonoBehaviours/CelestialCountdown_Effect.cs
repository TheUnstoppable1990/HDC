﻿using System;
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
using ModdingUtils.MonoBehaviours;

namespace HDC.MonoBehaviours
{
    public class CelestialCountdown_Effect_2 : MonoBehaviour
    {
        public SoundEvent soundUpgradeChargeLoop;

        private bool soundChargeIsPlaying;

        private float soundCounterLast;

        private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

        [Range(0f, 1f)]
        public float counter;

        public float timeToFill = 5f;

        public float timeToEmpty = 1f;

        public float duration = 1;

        public float hpMultiplier = 2f;

        public int upgradeLevel = 0;

        public int currentUpgradeLevel = 0;

        public ProceduralImage outerRing;

        public ProceduralImage fill;

        public Transform rotator;

        public Transform still;

        private CharacterData data;

        public GameObject[] upgradeObjects;

        private float remainingDuration;

        //private bool isUpgrading;

        private bool isCelestialForm;

        private float startCounter;

        public Gun gun;
        public GunAmmo gunAmmo;
        public CharacterStatModifiers characterStats;
        public Gravity gravity;

        public float defaultRLTime;
        private float reloadModifier = 0.001f;
        private float aSpeedModifier = 0.25f;

        public Player player;

        private CelestialGlow celestialGlow = null;
        public void Start()
        {
            this.soundCounterLast = this.counter;
            this.data = base.GetComponentInParent<CharacterData>();
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
            base.GetComponentInParent<ChildRPC>().childRPCs.Add("Celestial", new Action(this.RPCA_Activate));
        }

        public void OnDestroy()
        {
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
            base.GetComponentInParent<ChildRPC>().childRPCs.Remove("Celestial");
            this.SoundStop();
        }

        public void OnDisable()
        {
            this.SoundStop();
        }

        private void SoundPlay()
        {
            if (!this.soundChargeIsPlaying)
            {
                this.soundChargeIsPlaying = true;
                SoundManager.Instance.Play(this.soundUpgradeChargeLoop, base.transform, new SoundParameterBase[]
                {
                this.soundParameterIntensity
                });
            }
        }

        private void SoundStop()
        {
            if (this.soundChargeIsPlaying)
            {
                this.soundChargeIsPlaying = false;
                SoundManager.Instance.Stop(this.soundUpgradeChargeLoop, base.transform, true);
            }
        }

        private void ResetStuff()
        {
            this.SoundStop();
            this.remainingDuration = 0f;
            this.counter = 0f;
            this.upgradeLevel = 0;
            this.currentUpgradeLevel = 0;
            if (this.isCelestialForm)
            {

                this.isCelestialForm = false;                
            }
            this.SoundStop();
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
                //this.characterStats.sizeMultiplier *= 2f;
                celestialGlow = player.gameObject.AddComponent<CelestialGlow>();
            }
            else
            {
                this.data.maxHealth /= this.hpMultiplier;
                this.data.health /= this.hpMultiplier;
                this.gunAmmo.reloadTimeMultiplier = this.defaultRLTime;
                this.gun.attackSpeed /= this.aSpeedModifier;

                this.gravity.gravityForce *= 2f;
                this.characterStats.movementSpeed /= 2f;
                //this.characterStats.sizeMultiplier /= 2f;
                if (celestialGlow != null)
                {
                    UnityEngine.Object.Destroy(celestialGlow);
                    celestialGlow = null;
                }
            }
            this.ConfigureMassAndSize();
        }

        private void RPCA_Activate()
        {
            //this.upgradeLevel += 1;
            this.remainingDuration = this.duration;
        }

        private void Update()
        {
            if (this.soundCounterLast < this.counter)
            {
                this.SoundPlay();
            }
            else
            {
                this.SoundStop();
            }
            this.soundCounterLast = this.counter;
            this.soundParameterIntensity.intensity = this.counter;
            this.outerRing.fillAmount = this.counter;
            this.fill.fillAmount = this.counter;
            this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));
            if (!((bool)this.data.playerVel.GetFieldValue("simulated")))
            {
                this.startCounter = 1f;
                return;
            }
            this.startCounter -= TimeHandler.deltaTime;
            if (this.startCounter > 0f)
            {
                return;
            }
            if (this.remainingDuration > 0f)
            {
                if (!this.isCelestialForm)
                {
                    //Buffed Stats Go Here
                    this.alterStats(true);
                    this.isCelestialForm = true;
                    UnityEngine.Debug.Log("Celestial Form Active");                    
                }
                this.remainingDuration -= TimeHandler.deltaTime;  //reduces Duration here
                this.counter = this.remainingDuration / this.duration;  
                return; //Breaks before futher conditionals
            }
            if (this.isCelestialForm)
            {
                //Remove Buffs here
                this.alterStats(false);
                this.isCelestialForm = false;
                UnityEngine.Debug.Log("Celestial Form Disabled");
            }
            try
            {
                if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down)
                {
                    this.counter += TimeHandler.deltaTime / this.timeToFill;
                }
                else
                {
                    this.counter -= TimeHandler.deltaTime / this.timeToEmpty;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("First Catch");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                this.counter = Mathf.Clamp(this.counter, -0.1f / this.timeToFill, 1f);
                if (this.counter >= 1f && this.data.view.IsMine)
                {
                    this.remainingDuration = this.duration;
                    base.GetComponentInParent<ChildRPC>().CallFunction("Celestial");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Second Catch");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                if (this.counter <= 0f)
                {
                    this.rotator.gameObject.SetActive(false);
                    this.still.gameObject.SetActive(false);
                    return;
                }
                this.rotator.gameObject.SetActive(true);
                this.still.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Last Catch");
                UnityEngine.Debug.LogException(e);
            }
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

        private CelestialGlow celestialGlow = null;

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
                //this.characterStats.sizeMultiplier *= 2f;
                celestialGlow = player.gameObject.AddComponent<CelestialGlow>();
            }
            else
            {
                this.data.maxHealth /= this.hpMultiplier;
                this.data.health /= this.hpMultiplier;
                this.gunAmmo.reloadTimeMultiplier = this.defaultRLTime;
                this.gun.attackSpeed /= this.aSpeedModifier;

                this.gravity.gravityForce *= 2f;
                this.characterStats.movementSpeed /= 2f;
                //this.characterStats.sizeMultiplier /= 2f;
                if(celestialGlow != null)
                {
                    UnityEngine.Object.Destroy(celestialGlow);
                    celestialGlow = null;
                }
            }
            this.ConfigureMassAndSize();
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

    public class CelestialGlow : ReversibleEffect //Thanks Pykess for this Utility
    {
        private readonly Color color = new Color(0.75f, 1f, 1f, 0.75f); //light bluish i think?
        private ReversibleColorEffect colorEffect = null;

        public override void OnOnEnable()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }
        public override void OnStart()
        {
            this.colorEffect = base.player.gameObject.AddComponent<ReversibleColorEffect>();
            this.colorEffect.SetColor(this.color);
            this.colorEffect.SetLivesToEffect(1);
        }
        public override void OnOnDisable()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }
        public override void OnOnDestroy()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }

    }
}
