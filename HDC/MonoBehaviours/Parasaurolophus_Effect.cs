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
using HDC.Utilities;

namespace HDC.MonoBehaviours
{
    class Parasaurolophus_Effect : MonoBehaviour
    {
		public static float baseRange = 5f;
		public float rangeOfEffect = 5f;
		private Player player;
		private CharacterData data;
		private PanicAura pa;
		public float panic_speed = 0.5f;
		public float panic_regen = 0.25f;
		public float panic_block_cd = -0.25f;
		private float timePass = 0f;

		private List<Player> enemiesInRange = new List<Player>();
		private List<Player> alliesInRange = new List<Player>();

		private PanicGlow panicGlow = null;

		private void Start()
		{
			this.pa = this.player.gameObject.AddComponent<PanicAura>();
		}
		public void Destroy()
		{
			Destroy(this.pa);
			Destroy(this);			
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
			this.timePass += Time.deltaTime;
			if (enemiesInRange.Count > 0)
			{				
				if(panicGlow == null)
                {
					panicGlow = player.gameObject.GetOrAddComponent<PanicGlow>();
					panicGlow.panic_speed = panic_speed;
					panicGlow.panic_regen = panic_regen;
					panicGlow.panic_block_cd = panic_block_cd;
                }
				if(timePass > 1)
                {
					this.data.healthHandler.Heal(panic_regen * this.data.maxHealth);
					timePass = 0;
                }
			}
            else
            {
				if (panicGlow != null)
                {
					Destroy(panicGlow);
					panicGlow = null;
                }
            }
			
			
		}

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

		// Token: 0x06000052 RID: 82 RVA: 0x00003B30 File Offset: 0x00001D30
		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}


		public class PanicAura : MonoBehaviour
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
						new GradientAlphaKey(0.5f, 0f)
					},
					colorKeys = new GradientColorKey[]
					{
						new GradientColorKey(Color.green, 0f)
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
		public class PanicGlow : ReversibleEffect //Thanks Pykess for this Utility
		{
			private readonly Color color = Color.green; //light yellowish i think?
			private ReversibleColorEffect colorEffect = null;
			public float panic_speed = 0.5f;
			public float panic_block_cd = -0.25f;
			public float panic_regen = 10f;
			//public CharacterData charData;

			public override void OnOnEnable()
			{

				if (this.colorEffect != null)
				{
					this.colorEffect.Destroy();
				}
			}
			public override void OnStart()
			{	
				UnityEngine.Debug.Log($"Panic Speed is {panic_speed}, Panic Regen is {panic_regen*100}%/s, Panic Block Cooldown is {panic_block_cd}");
				this.characterStatModifiersModifier.movementSpeed_mult = (1f + panic_speed);
				this.blockModifier.cdMultiplier_mult = (1f + panic_block_cd);
				//this.characterStatModifiersModifier.regen_add = panic_regen;		


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
}
