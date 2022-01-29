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

