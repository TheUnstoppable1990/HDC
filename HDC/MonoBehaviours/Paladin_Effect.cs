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
using HDC.Cards;

namespace HDC.MonoBehaviours
{
	class Paladin_Effect : MonoBehaviour
	{
		public static float baseRange = 10f;
		public Player player;
		public CharacterData data;
		public float rangeOfEffect = 10f;		
		private float multiplier = Paladin.regen_percentage; //0.1f;
		  
		private float timePass = 0f;
		private float healAmount = 0f;
		private int healPerEnemy = Paladin.regen_amount;
		private float allyRatio = 0.1f;
		private PaladinAura pa;



		private void Start()
		{
			pa = player.gameObject.AddComponent<PaladinAura>();
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
			
		}

		private void OnDisable()
		{

		}

		private void Update()
		{

			//rangeOfEffect = pa.holyEffect.GetComponentInChildren<LineEffect>();
			rangeOfEffect = AssetTools.GetLineEffectRadius(pa.holyEffect.GetComponentInChildren<LineEffect>());
			//HDC.Log($"Radius is {rangeOfEffect}");

			List<Player> enInRange = GetLivingEnemyPlayersInRange(rangeOfEffect);
			List<Player> alInRange = GetLivingAllyPlayersInRange(rangeOfEffect);
			//float numOfEnemies = enInRange.Count;
			
			this.timePass += Time.deltaTime;
			if (timePass > 1f) //second counter
			{
				//this.healAmount = (float)enInRange.Count * this.multiplier * this.data.maxHealth;
				healAmount = (float)enInRange.Count * healPerEnemy;
				data.healthHandler.Heal(healAmount);//heal for each enemy in range
				if(alInRange.Count > 0)
				{
					if (data.health > data.maxHealth * (0.5f + allyRatio * alInRange.Count)) //health would be at least over half after healing each ally once
					{						
						foreach (Player ally in alInRange)
						{
							CharacterData aData = ally.GetComponent<CharacterData>();
							
							if (aData.health < aData.maxHealth)
							{
								float allyHealAmount = data.maxHealth * allyRatio;
								aData.healthHandler.Heal(allyHealAmount);
								Vector2 damage = new Vector2(0f, -1f * allyHealAmount);								
								//data.healthHandler.DoDamage(damage, data.playerVel.position, Color.cyan, null, null, false, false, true);
								data.healthHandler.CallTakeDamage(damage, data.playerVel.position);
							}
						}
					}
				}
				timePass = 0f;
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


		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}

		public List<Player> GetAllyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID == this.player.teamID && player.playerID != this.player.playerID
					select player).ToList<Player>();
		}

		
		public List<Player> GetOtherPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.playerID != this.player.playerID
					select player).ToList<Player>();
		}

		
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
				componentInChildren.radius = baseRange;
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

