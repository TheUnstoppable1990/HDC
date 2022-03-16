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
		public static float baseRange = 10f;
		public Player player;
		public CharacterData data;
		public float rangeOfEffect = 10f;
		//private List<Player> enemiesInRange = new List<Player>();
		//private List<Player> alliesInRange = new List<Player>();
		//private int numOfEnemies = 0;
		//private int previousNumOfEnemies = 0;
		//private float startHealth = 100f;
		//private float startMaxHealth = 100f;
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
			//this.startHealth = this.data.health;
			//this.startMaxHealth = this.data.maxHealth;
		}

		private void OnDisable()
		{
			//this.data.health = this.startHealth;
			//this.data.maxHealth = this.startMaxHealth;
		}

		private void Update()
		{
			List<Player> enInRange = GetLivingEnemyPlayersInRange(this.rangeOfEffect);
			List<Player> alInRange = GetLivingAllyPlayersInRange(this.rangeOfEffect);
			//float numOfEnemies = enInRange.Count;
			
			this.timePass += Time.deltaTime;
			if (this.timePass > 1f) //second counter
			{
				this.healAmount = (float)enInRange.Count * this.multiplier * this.data.maxHealth; 
				this.data.healthHandler.Heal(this.healAmount);//heal for each enemy in range
				if(alInRange.Count > 0)
				{
					if (this.data.health > this.data.maxHealth * (0.5f + this.allyRatio))
					{						
						foreach (Player ally in alInRange)
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
			//this.previousNumOfEnemies = this.numOfEnemies;
		}

		private List<Player> GetLivingEnemyPlayersInRange(float range)
		{
			return (
				from player in PlayerManager.instance.players
				where player.teamID != this.player.teamID && CanSee(this.player, player) && InRange(this.player, player, range) && IsAlive(player)
				select player
				).ToList<Player>();
		}
		private List<Player> GetLivingAllyPlayersInRange(float range)
        {
			return (
				from player in PlayerManager.instance.players
				where player.teamID == this.player.teamID && CanSee(this.player, player) && InRange(this.player, player, range) && IsAlive(player) && player.playerID != this.player.playerID
				select player
				).ToList<Player>();
		}
		private bool CanSee(Player myPlayer, Player otherPlayer)
		{
			return PlayerManager.instance.CanSeePlayer(myPlayer.transform.position, otherPlayer).canSee;
		}
		private bool InRange(Player myPlayer, Player otherPlayer, float range)
		{
			float num = Vector2.Distance(myPlayer.data.playerVel.position, otherPlayer.data.playerVel.position);
			return (num <= range);
		}
		private bool IsAlive(Player player)
		{
			return !player.data.dead;
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
				componentInChildren.radius = baseRange / HDC.auraConst;
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

