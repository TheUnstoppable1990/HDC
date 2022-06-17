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
using ModdingUtils.MonoBehaviours;
using ModdingUtils.Extensions;

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

        public ProceduralImage outerRing;
        public ProceduralImage fill;
        public Transform rotator;
        public Transform still;
        private CharacterData data;
        private float remainingDuration;
        private bool isCelestialForm;
        private float startCounter;
        public Gun gun;
        public GunAmmo gunAmmo;
        public CharacterStatModifiers characterStats;
        public Gravity gravity;

        public float defaultRLTime;
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
            healthHandler.reviveAction = (Action)Delegate.Remove(healthHandler.reviveAction, new Action(this.ResetStuff));
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
            if (this.isCelestialForm)
            {
                this.isCelestialForm = false;
                this.alterStats(false);
            }
            this.SoundStop();
        }
        private void alterStats(bool enable)
        {
            if (enable)
            {                
                celestialGlow = player.gameObject.AddComponent<CelestialGlow>();             

            }
            else
            {
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
                HDC.Log("First Catch");
                HDC.LogException(e);
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
                HDC.Log("Second Catch");
                HDC.LogException(e);
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
                HDC.Log("Last Catch");
                HDC.LogException(e);
            }
        }
        public void Destroy()
        {
            ResetStuff();
            UnityEngine.Object.Destroy(this);
        }
        private void ConfigureMassAndSize()
        {
            base.transform.localScale = Vector3.one * 1.2f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.2f) * 1f;//size multiplier? not sure if static or not
        }

    }
    
    public class CelestialGlow : ModdingUtils.MonoBehaviours.ReversibleEffect //Thanks Pykess for this Utility
    {
        private readonly Color color = new Color(1f, 1f, 0.25f, 0.75f); //light yellowish i think?
        private ReversibleColorEffect colorEffect = null;        
        private float dmgMultiplier = 2f;
        private float reloadModifier = 0.001f;
        private float aSpeedModifier = 0.125f;
        public CharacterData charData;
        
        public override void OnOnEnable()
        {
            
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }
        public override void OnStart()
        {
            this.gunStatModifier.damage_mult = this.dmgMultiplier;
            this.gunAmmoStatModifier.reloadTimeMultiplier_mult = this.reloadModifier;
            this.gunStatModifier.attackSpeed_mult = this.aSpeedModifier;            
            this.gravityModifier.gravityForce_mult = 0.5f;            
            this.characterStatModifiersModifier.movementSpeed_mult = 2f;


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
