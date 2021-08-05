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

namespace HDC.MonoBehaviours
{
    class Regeneration : MonoBehaviour
    {
        internal Player player;
        internal Gun gun;
        internal GunAmmo gunAmmo;
        internal Gravity gravity;
        internal HealthHandler health;
        internal CharacterData data;
        internal Block block;

        private Vector2 lastPosition;
        //private int count = 0;
        //private float distChange = 0.00f;
        private float timePass = 0.0f;
        private int secondCount = 0;

        private float healAmount = 5.0f;
        public float healRatio = 0.05f; //5 base hp/sec for base health of 100

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?
        }
        private void OnDestroy()
        {
            HealthHandler healthHandler = this.data.healthHandler;
            healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff)); //Adds a reset to character on revive?
            
        }
        public void Awake()
        {
            this.player = gameObject.GetComponent<Player>();
            this.gun = this.player.GetComponent<Holding>().holdable.GetComponent<Gun>();
            this.data = this.player.GetComponent<CharacterData>();
            this.health = this.player.GetComponent<HealthHandler>();
            this.gravity = this.player.GetComponent<Gravity>();
            this.block = this.player.GetComponent<Block>();
            this.gunAmmo = this.gun.GetComponentInChildren<GunAmmo>();
            lastPosition = this.data.playerVel.position;
        }


       //Should the Health Boost be exponential? Right now will be 1hp per second (if at base health of 100)
        private void Update()
        {
            if (this.data.input.direction == Vector3.zero || this.data.input.direction == Vector3.down)
            {
                timePass += Time.deltaTime;
                if (timePass > 1.0f)  //every second
                {
                    healAmount = (healRatio + secondCount / 100f) * this.data.maxHealth;
                    this.data.health += healAmount;
                    if (this.data.health > this.data.maxHealth)
                    {
                        this.data.health = this.data.maxHealth; // simpler way to cap health
                    }
                    timePass = 0.0f;
                    secondCount++;
                    UnityEngine.Debug.Log(healAmount);
                }
            }
            else
            { 
                timePass = 0.0f;
                secondCount = 0;
            }
            
        }
        private void ResetStuff()
        {
            timePass = 0.0f;
        }
    }
}
