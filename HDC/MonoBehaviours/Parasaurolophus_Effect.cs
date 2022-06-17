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
		public float rangeOfEffect = baseRange;
		private Player player;
		private CharacterData data;
		private PanicAura pa;
		public float panic_speed = 0f;
		public float panic_regen = 0f;
		public float panic_block_cd = 0f;
		private float timePass = 0f;
	

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
			List<Player> enInRange = GetLivingEnemyPlayersInRange(this.rangeOfEffect);			
			this.timePass += Time.deltaTime;
			if (enInRange.Count > 0)
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
		private List<Player> GetLivingEnemyPlayersInRange(float range)
		{
			return (
				from player in PlayerManager.instance.players
				where player.teamID != this.player.teamID && CanSee(this.player, player) && InRange(this.player,player,range) && IsAlive(player)
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
		public void AdjustSize(int num)
        {
			if(pa != null)
            {
				pa.AdjustSize(num);
				this.rangeOfEffect = baseRange * num;
            }			
        }

		public class PanicAura : MonoBehaviour
		{
			private static GameObject lineEffect;
			private Player player = null;
			public GameObject panicEffect = null;
			public GameObject panicObj = null;
			private LineEffect panicLineEffect;

			public void Start()
			{
				this.player = base.gameObject.GetComponent<Player>();
				this.panicObj = new GameObject();
				this.panicObj.transform.SetParent(this.player.transform);
				this.panicObj.transform.position = this.player.transform.position;
				if (lineEffect == null)
				{
					lineEffect = AssetTools.GetLineEffect("Lifestealer");
				}
				this.panicEffect = UnityEngine.Object.Instantiate<GameObject>(lineEffect, this.panicObj.transform);
				this.panicLineEffect = this.panicEffect.GetComponentInChildren<LineEffect>();
				this.panicLineEffect.colorOverTime = new Gradient
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
				this.panicLineEffect.widthMultiplier = 1f;
				this.panicLineEffect.radius = baseRange / HDC.auraConst;
				this.panicLineEffect.raycastCollision = true;
				this.panicLineEffect.useColorOverTime = true;
			}

			public void Destroy()
			{
				UnityEngine.Object.Destroy(this.panicObj);
				UnityEngine.Object.Destroy(this.panicEffect);
				UnityEngine.Object.Destroy(this);
			}
			public void AdjustSize(int num)
            {
				HDC.Log($"Adjusting size for {num} panic aura(s)");
				this.panicLineEffect.radius = baseRange * num / HDC.auraConst;
            }


		}
		public class PanicGlow : ModdingUtils.MonoBehaviours.ReversibleEffect //Thanks Pykess for this Utility
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
				HDC.Log($"Panic Speed is {panic_speed}, Panic Regen is {panic_regen*100}%/s, Panic Block Cooldown is {panic_block_cd}");
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
