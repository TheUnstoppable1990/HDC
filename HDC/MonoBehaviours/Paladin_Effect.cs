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
using HDC.Utilities;

namespace HDC.MonoBehaviours
{

	/*
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
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
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

        private void ConfigureMassAndSize()
        {
            base.transform.localScale = Vector3.one * 1.2f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.2f) * 1f;//size multiplier? not sure if static or not
        }
    }
    */
	class Paladin_Effect : MonoBehaviour
	{
		// Token: 0x0600004B RID: 75 RVA: 0x000036E6 File Offset: 0x000018E6
		private void Start()
		{
			this.pa = this.player.gameObject.AddComponent<Paladin_Effect.PaladinAura>();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000036FF File Offset: 0x000018FF
		private void OnDestroy()
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003702 File Offset: 0x00001902
		public void Destroy()
		{
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000370C File Offset: 0x0000190C
		private void Awake()
		{
			this.player = base.gameObject.GetComponent<Player>();
			this.data = this.player.GetComponent<CharacterData>();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003731 File Offset: 0x00001931
		private void OnEnable()
		{
			this.startHealth = this.data.health;
			this.startMaxHealth = this.data.maxHealth;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003756 File Offset: 0x00001956
		private void OnDisable()
		{
			this.data.health = this.startHealth;
			this.data.maxHealth = this.startMaxHealth;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000377C File Offset: 0x0000197C
		private void Update()
		{
			foreach (Player player in this.GetOtherPlayers())
			{
				CharacterData component = player.GetComponent<CharacterData>();
				float num = Vector2.Distance(this.data.playerVel.position, component.playerVel.position);
				foreach (Player player2 in this.enemiesInRange)
				{
					CharacterData component2 = player2.GetComponent<CharacterData>();
					bool dead = component2.dead;
					if (dead)
					{
						this.enemiesInRange.Remove(player2);
					}
				}
				bool canSee = PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee;
				bool flag = num < this.rangeOfEffect && canSee;
				if (flag)
				{
					bool flag2 = this.player.teamID == player.teamID;
					if (flag2)
					{
						bool flag3 = !this.alliesInRange.Contains(player);
						if (flag3)
						{
							this.alliesInRange.Add(player);
						}
					}
					else
					{
						bool flag4 = !this.enemiesInRange.Contains(player);
						if (flag4)
						{
							this.enemiesInRange.Add(player);
						}
					}
				}
				else
				{
					bool flag5 = this.player.teamID == player.teamID;
					if (flag5)
					{
						bool flag6 = this.alliesInRange.Contains(player);
						if (flag6)
						{
							this.alliesInRange.Remove(player);
						}
					}
					else
					{
						bool flag7 = this.enemiesInRange.Contains(player);
						if (flag7)
						{
							this.enemiesInRange.Remove(player);
						}
					}
				}
			}
			this.numOfEnemies = this.enemiesInRange.Count;
			this.timePass += Time.deltaTime;
			bool flag8 = this.timePass > 1f;
			if (flag8)
			{
				this.healAmount = (float)this.enemiesInRange.Count * this.multiplier * this.data.maxHealth;
				this.data.healthHandler.Heal(this.healAmount);
				bool flag9 = this.alliesInRange.Count > 0;
				if (flag9)
				{
					bool flag10 = this.data.health > this.data.maxHealth * (0.5f + this.allyRatio);
					if (flag10)
					{
						foreach (Player player3 in this.alliesInRange)
						{
							CharacterData component3 = player3.GetComponent<CharacterData>();
							bool flag11 = component3.health < component3.maxHealth;
							if (flag11)
							{
								float num2 = this.data.maxHealth * this.allyRatio;
								component3.healthHandler.Heal(num2);
								Vector2 damage = new Vector2(0f, -1f * num2);
								this.data.healthHandler.DoDamage(damage, this.data.playerVel.position, Color.cyan, null, null, false, false, true);
							}
						}
					}
				}
				this.timePass = 0f;
			}
			this.previousNumOfEnemies = this.numOfEnemies;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003B30 File Offset: 0x00001D30
		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003B64 File Offset: 0x00001D64
		public List<Player> GetAllyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID == this.player.teamID && player.playerID != this.player.playerID
					select player).ToList<Player>();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003B98 File Offset: 0x00001D98
		public List<Player> GetOtherPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.playerID != this.player.playerID
					select player).ToList<Player>();
		}

		// Token: 0x04000048 RID: 72
		public static float baseRange = 10f;

		// Token: 0x04000049 RID: 73
		public Player player;

		// Token: 0x0400004A RID: 74
		public CharacterData data;

		// Token: 0x0400004B RID: 75
		public float rangeOfEffect = 10f;

		// Token: 0x0400004C RID: 76
		private List<Player> enemiesInRange = new List<Player>();

		// Token: 0x0400004D RID: 77
		private List<Player> alliesInRange = new List<Player>();

		// Token: 0x0400004E RID: 78
		private int numOfEnemies = 0;

		// Token: 0x0400004F RID: 79
		private int previousNumOfEnemies = 0;

		// Token: 0x04000050 RID: 80
		private float startHealth = 100f;

		// Token: 0x04000051 RID: 81
		private float startMaxHealth = 100f;

		// Token: 0x04000052 RID: 82
		private float multiplier = 0.15f;

		// Token: 0x04000053 RID: 83
		private float timePass = 0f;

		// Token: 0x04000054 RID: 84
		private float healAmount = 0f;

		// Token: 0x04000055 RID: 85
		private float allyRatio = 0.1f;

		// Token: 0x04000056 RID: 86
		private Paladin_Effect.PaladinAura pa;

		// Token: 0x0200002A RID: 42
		public class PaladinAura : MonoBehaviour
		{
			// Token: 0x06000116 RID: 278 RVA: 0x00005A28 File Offset: 0x00003C28
			public void Start()
			{
				this.player = base.gameObject.GetComponent<Player>();
				this.holyLightObj = new GameObject();
				this.holyLightObj.transform.SetParent(this.player.transform);
				this.holyLightObj.transform.position = this.player.transform.position;
				bool flag = Paladin_Effect.PaladinAura.lineEffect == null;
				if (flag)
				{
					Paladin_Effect.PaladinAura.lineEffect = AssetTools.GetLineEffect("Lifestealer");
				}
				this.holyEffect = UnityEngine.Object.Instantiate<GameObject>(Paladin_Effect.PaladinAura.lineEffect, this.holyLightObj.transform);
				LineEffect componentInChildren = this.holyEffect.GetComponentInChildren<LineEffect>();
				componentInChildren.colorOverTime = new Gradient
				{
					alphaKeys = new GradientAlphaKey[]
					{
						new GradientAlphaKey(1f, 0f)
					},
					colorKeys = new GradientColorKey[]
					{
						new GradientColorKey(HLConst.color, 0f)
					},
					mode = GradientMode.Fixed
				};
				componentInChildren.widthMultiplier = 1f;
				componentInChildren.radius = Paladin_Effect.baseRange / 1.5f;
				componentInChildren.raycastCollision = true;
				componentInChildren.useColorOverTime = true;
			}

			// Token: 0x06000117 RID: 279 RVA: 0x00005B5B File Offset: 0x00003D5B
			public void Destroy()
			{
				UnityEngine.Object.Destroy(this.holyLightObj);
				UnityEngine.Object.Destroy(this.holyEffect);
				UnityEngine.Object.Destroy(this);
			}

			// Token: 0x040000A7 RID: 167
			private static GameObject lineEffect;

			// Token: 0x040000A8 RID: 168
			private Player player = null;

			// Token: 0x040000A9 RID: 169
			public GameObject holyEffect = null;

			// Token: 0x040000AA RID: 170
			public GameObject holyLightObj = null;
		}
	}
}
}
