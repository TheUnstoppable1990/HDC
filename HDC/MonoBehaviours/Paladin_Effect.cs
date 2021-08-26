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
    class Paladin_Effect : MonoBehaviour
    {
        public Player player;
        public CharacterData data;
        public float rangeOfEffect = 10f;
        private List<Player> enemiesInRange = new List<Player>();
        private List<Player> alliesInRange = new List<Player>();
        private int numOfEnemies = 0;
        private int previousNumOfEnemies = 0;
        private float startHealth = 100f;
        private float startMaxHealth = 100f;
        //private float healthChange = 1f;
        private float multiplier = 0.15f;
        private float timePass = 0.0f;
        private float healAmount = 0.0f;
        private float allyRatio = 0.10f;
        private void Start()
        {

        }
        private void OnDestroy()
        {

        }
        private void Awake()
        {
            this.player = gameObject.GetComponent<Player>(); //might make the player setting in the card redundant but oh well at this point
            this.data = this.player.GetComponent<CharacterData>();

        }
        private void OnEnable()
        {
            this.startHealth = this.data.health;
            this.startMaxHealth = this.data.maxHealth;

            UnityEngine.Debug.Log($"Starting health is {this.startHealth}/{this.startMaxHealth}");
        }
        private void OnDisable()
        {
            //resets health on disable?
            this.data.health = this.startHealth;
            this.data.maxHealth = this.startMaxHealth;
        }
        private void Update()
        {
            //ConsoleLog(this.data);
            foreach (Player otherPlayer in GetOtherPlayers())
            {
                CharacterData otherData = otherPlayer.GetComponent<CharacterData>();
                float dist = Vector2.Distance(this.data.playerVel.position, otherData.playerVel.position);
                //UnityEngine.Debug.Log(dist);
                foreach(Player enemy in this.enemiesInRange)
                {
                    CharacterData enemyData = enemy.GetComponent<CharacterData>();
                    if (enemyData.dead)//first attempt at correcting the dead thing
                    {
                        this.enemiesInRange.Remove(enemy);
                    }
                }
                if(dist < rangeOfEffect)
                {
                    //Player is in range. Is it Friend or Foe?
                    if(this.player.teamID == otherPlayer.teamID)
                    {
                        //Ok it's a friend
                        if (!this.alliesInRange.Contains(otherPlayer))//only if it's not already on the list
                        {                            
                            this.alliesInRange.Add(otherPlayer);
                        }
                    }
                    else
                    {
                        //Ok not a friend!
                        if (!this.enemiesInRange.Contains(otherPlayer))//only if it's not already on the list
                        {
                            this.enemiesInRange.Add(otherPlayer);
                        }
                    }
                }
                else
                {
                    if (this.player.teamID == otherPlayer.teamID)
                    {
                        if (this.alliesInRange.Contains(otherPlayer))
                        {
                            this.alliesInRange.Remove(otherPlayer);
                        }
                    }
                    else
                    {
                        if (this.enemiesInRange.Contains(otherPlayer))
                        {
                            this.enemiesInRange.Remove(otherPlayer);
                        }
                    }
                }

                
            }
            this.numOfEnemies = this.enemiesInRange.Count;


            // start for new code
            this.timePass += Time.deltaTime;
            if (this.timePass > 1.0f)  //every second
            {
                //primary efect of healing self
                healAmount = this.enemiesInRange.Count * multiplier * this.data.maxHealth;
                this.data.health += healAmount;

                //secondary effect of healing allies
                if(this.alliesInRange.Count > 0)
                {
                    if(this.data.health > (this.data.maxHealth * (0.5f + this.allyRatio)))//always more than half health after ally healing
                    {
                        foreach(Player ally in alliesInRange)
                        {
                            CharacterData allyData = ally.GetComponent<CharacterData>();
                            if(allyData.health < allyData.maxHealth)
                            {
                                allyData.health += this.data.maxHealth * this.allyRatio;
                                this.data.health -= this.data.maxHealth * this.allyRatio;
                                if(allyData.health > allyData.maxHealth)
                                {
                                    this.data.health += (allyData.health - allyData.maxHealth);// corrects for over healing
                                    allyData.health = allyData.maxHealth;
                                }
                            }
                        }
                    }
                }

                this.timePass = 0.0f; //resets the second
            }

                /*
                int diff = Math.Abs(this.numOfEnemies - this.previousNumOfEnemies);
                this.healthChange = (float)Math.Pow(multiplier, diff);
                if (this.numOfEnemies > this.previousNumOfEnemies)
                {
                    this.data.maxHealth *= this.healthChange;
                    this.data.health *= this.healthChange;
                    this.ConfigureMassAndSize();
                    /*
                    foreach(Player ally in this.alliesInRange)// this won't work except when enemy numbers change ally entrance will totally be borked
                    {
                        //boosts Ally stats
                        CharacterData allyData = ally.GetComponent<CharacterData>();
                        allyData.maxHealth *= this.healthChange;
                        allyData.health *= this.healthChange;
                    }
                    UnityEngine.Debug.Log($"Start Health: {this.startMaxHealth}, Current Health: {this.data.maxHealth}");
                }
                else if (this.numOfEnemies < this.previousNumOfEnemies)
                {
                    this.data.maxHealth /= this.healthChange;
                    this.data.health /= this.healthChange;
                    this.ConfigureMassAndSize();
                    /*
                    foreach (Player ally in this.alliesInRange)
                    {
                        //boosts Ally stats
                        CharacterData allyData = ally.GetComponent<CharacterData>();
                        allyData.maxHealth /= this.healthChange;
                        allyData.health /= this.healthChange;
                    }

                }*/

                
            this.previousNumOfEnemies = this.numOfEnemies;

            if (this.data.health > this.data.maxHealth)
            {
                this.data.health = this.data.maxHealth;
            }//checks at the end of the update that health isnt greater than max health
        }
        public List<Player> GetEnemyPlayers()
        {
            return PlayerManager.instance.players.Where(player => player.teamID != this.player.teamID).ToList();
        }
        public List<Player> GetAllyPlayers()
        {
            return PlayerManager.instance.players.Where(player => (player.teamID == this.player.teamID && player.playerID != this.player.playerID)).ToList();
        }
        public List<Player> GetOtherPlayers()
        {
            return PlayerManager.instance.players.Where(player => player.playerID != this.player.playerID).ToList();
        }
        private void ConsoleLog(CharacterData target)
        {
            UnityEngine.Debug.Log($"{target.playerVel.position.x},{target.playerVel.position.y}");
        }
        private void ConfigureMassAndSize()
        {
            base.transform.localScale = Vector3.one * 1.2f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.2f) * 1f;//size multiplier? not sure if static or not
        }
    }
}
