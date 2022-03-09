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
using HDC.Utilities;

namespace HDC.MonoBehaviours
{
	class Paladin_Effect : MonoBehaviour
	{
		public static float baseRange = 10f;
		public Player player;
		public CharacterData data;
		public float rangeOfEffect = 10f;
		private List<Player> enemiesInRange = new List<Player>();
		private List<Player> alliesInRange = new List<Player>();
		private int numOfEnemies = 0;
		private int previousNumOfEnemies = 0;
		private float startHealth = 100f;
		private float startMaxHealth = 100f;
		private float multiplier = 0.15f;
		private float timePass = 0f;
		private float healAmount = 0f;
		private float allyRatio = 0.1f;
		private Paladin_Effect.PaladinAura pa;

		private void Start()
		{
			this.pa = this.player.gameObject.AddComponent<Paladin_Effect.PaladinAura>();
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
			this.player = base.gameObject.GetComponent<Player>();
			this.data = this.player.GetComponent<CharacterData>();
		}

		private void OnEnable()
		{
			this.startHealth = this.data.health;
			this.startMaxHealth = this.data.maxHealth;
		}

		private void OnDisable()
		{
			this.data.health = this.startHealth;
			this.data.maxHealth = this.startMaxHealth;
		}

		private void Update()
		{
			foreach (Player player in this.GetOtherPlayers())
			{
				CharacterData pData = player.GetComponent<CharacterData>();
				float num = Vector2.Distance(this.data.playerVel.position, pData.playerVel.position);
				foreach (Player enemy in this.enemiesInRange)
				{
					CharacterData eData = enemy.GetComponent<CharacterData>();
					if (eData.dead)
					{
						this.enemiesInRange.Remove(enemy);
					}
				}
				bool canSee = PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee;				
				if (num < this.rangeOfEffect && canSee)
				{					
					if (this.player.teamID == player.teamID)
					{						
						if (!this.alliesInRange.Contains(player))
						{
							this.alliesInRange.Add(player);
						}
					}
					else
					{
						if (!this.enemiesInRange.Contains(player))
						{
							this.enemiesInRange.Add(player);
						}
					}
				}
				else
				{
					if (this.player.teamID == player.teamID)
					{
						if (this.alliesInRange.Contains(player))
						{
							this.alliesInRange.Remove(player);
						}
					}
					else
					{
						if (this.enemiesInRange.Contains(player))
						{
							this.enemiesInRange.Remove(player);
						}
					}
				}
			}
			this.numOfEnemies = this.enemiesInRange.Count;
			this.timePass += Time.deltaTime;
			if (this.timePass > 1f)
			{
				this.healAmount = (float)this.enemiesInRange.Count * this.multiplier * this.data.maxHealth;
				this.data.healthHandler.Heal(this.healAmount);
				if (this.alliesInRange.Count > 0)
				{

					if (this.data.health > this.data.maxHealth * (0.5f + this.allyRatio))
					{
						foreach (Player ally in this.alliesInRange)
						{
							CharacterData aData = ally.GetComponent<CharacterData>();
							if (aData.health < aData.maxHealth)
							{
								float healAmount = this.data.maxHealth * this.allyRatio;
								aData.healthHandler.Heal(healAmount);
								Vector2 damage = new Vector2(0f, -1f * healAmount);
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
		
		// Token: 0x0200002A RID: 42
		public class PaladinAura : MonoBehaviour
		{
			private static GameObject lineEffect;
			private Player player = null;
			public GameObject holyEffect = null;
			public GameObject holyLightObj = null;
			
			public void Start()
			{
				this.player = base.gameObject.GetComponent<Player>();
				this.holyLightObj = new GameObject();
				this.holyLightObj.transform.SetParent(this.player.transform);
				this.holyLightObj.transform.position = this.player.transform.position;
				if (lineEffect == null)
				{
					lineEffect = AssetTools.GetLineEffect("Lifestealer");
				}
				this.holyEffect = UnityEngine.Object.Instantiate<GameObject>(lineEffect, this.holyLightObj.transform);
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
				componentInChildren.radius = baseRange / 1.5f;
				componentInChildren.raycastCollision = true;
				componentInChildren.useColorOverTime = true;
			}

			public void Destroy()
			{
				UnityEngine.Object.Destroy(this.holyLightObj);
				UnityEngine.Object.Destroy(this.holyEffect);
				UnityEngine.Object.Destroy(this);
			}

			
		}
	}
}

